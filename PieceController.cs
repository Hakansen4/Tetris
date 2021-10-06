using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PieceController : MonoBehaviour
{
    GameController gamecontroller;
    public AudioClip  Move, Rotate;
    public AudioSource AudioController;
    float Timer = 0f;
    bool CanMove = true;
    public GameObject Rig;
    void Start()
    {
        AudioController = GetComponent<AudioSource>();
        gamecontroller = FindObjectOfType<GameController>();
        if(gamecontroller.isMuted)
        {
            AudioController.mute = true;
        }
    }
    void RegisterBlock()
    {
        foreach (Transform Block in Rig.transform)
        {
            if (Mathf.FloorToInt(Block.position.y) < GameController.Height)
                gamecontroller.Board[Mathf.FloorToInt(Block.position.x), Mathf.FloorToInt(Block.position.y)] = Block;
            else
                gamecontroller.isGameOver = true;
            //Debug.Log("x= "+Mathf.FloorToInt(Block.position.x) + " ve " +"y= "+ Mathf.FloorToInt(Block.position.y)+" Kaydedildi");
        }
    }
    bool checkValid()
    {
        foreach (Transform Block in Rig.transform)
        {
            if(Block.transform.position.x>=GameController.Width||
                Block.transform.position.x<0||
                Block.transform.position.y<0)
            { return false; }
            if (Block.transform.position.y < GameController.Height &&
                gamecontroller.Board[Mathf.FloorToInt(Block.transform.position.x), Mathf.FloorToInt(Block.transform.position.y)] != null)
            { return false; }
        }
        return true;
    }
    void Update()
    {
        if (!gamecontroller.isGameOver)
        {
            if (CanMove)
            {
                //Movement
                Timer += Time.deltaTime;
                //Move Down
                if ((Timer > GameController.DownMoveTimer && gamecontroller.MoveDown)   || (Input.GetKeyDown(KeyCode.DownArrow)) )        
                {
                    transform.position += new Vector3(0, -1, 0);
                    gamecontroller.MoveDown = false;
                    Timer = 0;
                    if (checkValid())
                    {
                        AudioController.clip = Move;
                        AudioController.Play();
                    }
                    if (!checkValid())
                    {
                        transform.position -= new Vector3(0, -1, 0);
                        CanMove = false;
                        RegisterBlock();
                        gamecontroller.ClearLine();
                        gamecontroller.GiveScore();
                        if (!gamecontroller.isGameOver)
                            gamecontroller.SpawnNewPiece();
                    }
                }
                //Drop
                if (Timer > gamecontroller.DropTimer)
                {
                    transform.position += new Vector3(0, -1, 0);
                    Timer = 0;
                    AudioController.clip = Move;
                    AudioController.Play();
                    if (!checkValid())
                    {
                        transform.position -= new Vector3(0, -1, 0);
                        RegisterBlock();
                        CanMove = false;
                        gamecontroller.ClearLine();
                        gamecontroller.GiveScore();
                        if (!gamecontroller.isGameOver)
                            gamecontroller.SpawnNewPiece();
                    }
                }
                //Move Left
                if((gamecontroller.MoveLeft)  || (Input.GetKeyDown(KeyCode.LeftArrow)))                    
                {
                    transform.position += new Vector3(-1, 0, 0);
                    gamecontroller.MoveLeft = false;
                    if (checkValid())
                    {
                        AudioController.clip = Move;
                        AudioController.Play();
                    }
                    if (!checkValid())
                    {
                        transform.position -= new Vector3(-1, 0, 0);
                    }
                }
                //Move Right
                else if((gamecontroller.MoveRight)    || (Input.GetKeyDown(KeyCode.RightArrow)))          
                {
                    transform.position += new Vector3(1, 0, 0);
                    gamecontroller.MoveRight = false;
                    if (checkValid())
                    {
                        AudioController.clip = Move;
                        AudioController.Play();
                    }
                    if (!checkValid())
                    {
                        transform.position -= new Vector3(1, 0, 0);
                    }
                }
                //Rotation
                if((gamecontroller.RotatePiece)   || (Input.GetKeyDown(KeyCode.UpArrow)))                          
                {
                    Rig.transform.eulerAngles += new Vector3(0, 0, 90);
                    gamecontroller.RotatePiece = false;
                    if (checkValid())
                    {
                        AudioController.clip = Rotate;
                        AudioController.Play();
                    }
                    if (!checkValid())
                    {
                        Rig.transform.eulerAngles -= new Vector3(0, 0, 90);
                    }
                }
                //Movement End
            }
        }
    }
}
