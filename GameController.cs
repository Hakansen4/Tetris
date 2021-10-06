using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GameController : MonoBehaviour
{
    #region Float
    public float DropTimer = 0.8f;
    #endregion
    #region Static Variables
    public static float DownMoveTimer = 0.05f;
    public static int Width = 10, Height = 20;
    public static float  spawnTimer = 0.8f;
#endregion
    #region Bool
    public bool isGameOver = false, isMuted = false;bool Startgame = false,Justone = true;bool IlkParcaOlustu = false, OneSec = false;
    public bool MoveLeft = false, MoveRight = false, MoveDown = false, RotatePiece = false;
    #endregion
    static float[] LvlSpeeds = new float[] { 0.8f, 0.716f, 0.63f, 0.55f, 0.46f, 0.38f,
        0.3f, 0.216f, 0.13f, 0.1f, 0.083f, 0.06f, 0.05f, 0.03f, 0.016f };
    #region İnt
    int CompletedLine = 0, RandomNextBlock = 0, RandomBlock = 0, HoldNumber = 9,
        Score = 0, HighScore = 0, SifirSayisi = 9, line = 0, level = 0, lineRange = 10,
        startTimeText=3, GameTimerMin, GameTimerSec;
    #endregion
    #region Strings
    string Sifirlar;public static string _HighScoreLvl = "HighScoreLvl"; public static string _HighScoreTimer = "HighScoreTimer"; public string GameMode;
    #endregion
    #region Audio
    AudioSource AudioController;
    public AudioClip LineClear;
    #endregion
    #region Text
    public TextMeshProUGUI StartTime,Scoretxt,BestScoretxt;
    public Text Point, levelText, lineText,TimerText;
    #endregion
    #region GameObjects
    public GameObject[] Pieces;public GameObject[] NextPieces;public GameObject[] HoldPieces;
    public GameObject PauseMenu, SettingsMenu, StartMenu,GameoverMenu;
    public Toggle SoundToggle; public Toggle MusicToggle;
    #endregion
    public Transform[,] Board = new Transform[Width, Height];
    public Transform NextTable, HoldTable;
    private void Awake()
    {
        AudioController = GetComponent<AudioSource>();
        if(SceneManager.GetActiveScene().name=="Level")
        {
            GameMode = "Level";
            HighScore = PlayerPrefs.GetInt(_HighScoreLvl);
        }
        else if (SceneManager.GetActiveScene().name == "Level_1")
        {
            GameMode = "Timer";
            GameTimerSec = 0;
            GameTimerMin = 2;
            HighScore = PlayerPrefs.GetInt(_HighScoreTimer);
        }
    }
    void Start()
    {
        CheckMutedSounds();
        CheckMutedMusic();
    }
    void StartGame()
    {
        float Timer = Time.timeSinceLevelLoad;
        if(Mathf.FloorToInt(Timer)==1)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            startTimeText = 2;
            StartTime.text = startTimeText.ToString();
        }
        else if(Mathf.FloorToInt(Timer)==2)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            startTimeText = 1;
            StartTime.text = startTimeText.ToString();
        }
        else if (Mathf.FloorToInt(Timer) == 3)
        {
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            startTimeText = 0;
            StartTime.text = startTimeText.ToString();
        }
        else if (Mathf.FloorToInt(Timer) == 4   &&  Justone)
        {
            StartMenu.SetActive(false);
            Startgame = true;
            Justone = false;
        }
    }
    void Update()
    {
        
        StartGame();
        if (Startgame)
        {
            SpawnNewPiece();
            IlkParcaOlustu = true;
            Startgame = false;
        }
        if(GameMode=="Level")
        LevelSpeedControl();
        if(GameMode=="Timer"  &&  IlkParcaOlustu    &&  !OneSec)
        {
            StartCoroutine(Timer());
        }
        if(isGameOver)
        {
            GameOver();
        }
    }
    public void GameOver()
    {
        if(GameMode=="Level")
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(_HighScoreLvl, HighScore);
            }
            Scoretxt.text = Score.ToString();
            BestScoretxt.text = HighScore.ToString();
            GameoverMenu.SetActive(true);
        }
        else
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                PlayerPrefs.SetInt(_HighScoreTimer, HighScore);
            }
            Scoretxt.text = Score.ToString();
            BestScoretxt.text = HighScore.ToString();
            GameoverMenu.SetActive(true);
        }
    }
    public void ClearLine()
    {
        for(int y=0;y<Height;y++)
        {
            if (IsLineComplete(y))
                CompletedLine++;
        }
        line += CompletedLine;
        lineText.text = line.ToString();
        if (CompletedLine==1)
        {
            for(int y=0;y<Height;y++)
            {
                if(IsLineComplete(y))
                {
                    DestroyLine(y);
                    MoveLines(y);
                    AudioController.clip = LineClear;
                    AudioController.Play();
                }
            }
        }
        else if (CompletedLine > 1)
        {
            int []NeedMove= new int[4];
            int i = 0;
            for (int y = 0; y < Height; y++)
            {
                if (IsLineComplete(y))
                {
                    DestroyLine(y);
                    NeedMove[i] = y;
                    i++;
                }
            }
            AudioController.clip = LineClear;
            AudioController.Play();
            for (int x=CompletedLine-1;x>=0;x--)
            {
                MoveLines(NeedMove[x]);
            }
        }
    }
    void MoveLines(int i)
    {
        for (int y = i+1; y < Height; y++) 
        {
            for (int x = 0; x < Width; x++)
            {
                if(Board[x,y]!=null)
                {
                    if (Board[x, y - 1] == null)
                    {
                        Board[x, y - 1] = Board[x, y];
                        Board[x, y - 1].gameObject.transform.position -= new Vector3(0, 1, 0);
                        Board[x, y] = null;
                        if (y >= 2 && Board[x, y - 2] == null) MoveLines(y - 1);
                    }
                }
            }
        }
    }
    void DestroyLine(int y)
    {
        for (int x = 0; x < Width; x++) 
        {
            Destroy(Board[x, y].gameObject);
            Board[x, y] = null;
        }
    }
    bool IsLineComplete(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if(Board[x,y]==null)
            {
                return false;
            }
        }
        return true;
    }
    public void GiveScore()
    {
        if(CompletedLine==1)
        {
            Score += 40;
        }
        else if(CompletedLine==2)
        {
            Score += 100;
        }
        else if(CompletedLine==3)
        {
            Score += 300;
        }
        else if(CompletedLine==4)
        {
            Score += 1200;
        }
        CompletedLine = 0;
        for(int i=0;i<9;i++)
        {
            if(Score.ToString().Length==i)
            {
                SifirSayisi = 9 - i;
            }
        }
        for(int i=0;i<SifirSayisi;i++)
        {
            Sifirlar += "0";
        }
        Point.text = Sifirlar + Score.ToString();
        Sifirlar = null;
    }
    public void SpawnNewPiece()
    {
        Invoke("spawn", 0.5f);
    }
    void LevelSpeedControl()
    {
        LevelUp();
        SpeedUp();
    }
    void SpeedUp()
    {
        if(level==11    ||  level==12)
        {
            DropTimer = LvlSpeeds[10];
        }
        else if(level == 14 ||  level==15)
        {
            DropTimer = LvlSpeeds[11];
        }
        else if (level == 17 || level == 18)
        {
            DropTimer = LvlSpeeds[12];
        }
        else if (level>19   &&  level<29)
        {
            DropTimer = LvlSpeeds[13];
        }
        else if (level >=29)
        {
            DropTimer = LvlSpeeds[14];
        }
        else
        {
            DropTimer = LvlSpeeds[level];
        }
    }
    IEnumerator Timer()
    {
        OneSec = true;
        yield return new WaitForSeconds(1);
        if (GameTimerMin>0   &&  GameTimerSec==0)
        {
            GameTimerMin--;
            GameTimerSec = 60;
        }
        else if(GameTimerMin==0 &&  GameTimerSec==0)
        {
            isGameOver = true;
        }
        if(!isGameOver)
        GameTimerSec--;
        if (GameTimerSec >= 10)
        {
            TimerText.text = "0" + GameTimerMin.ToString() + ":" + GameTimerSec;
        }
        else
        {
            TimerText.text = "0" + GameTimerMin.ToString() + ":0" + GameTimerSec;
        }
        OneSec = false;
    }
    void LevelUp()
    { 
        if(line==lineRange)
        {
            level++;
            lineRange += 10;
            levelText.text = level.ToString();
        }
    }
    public void spawn()
    {
        RandomBlock = Random.Range(0, 7);
        if (IlkParcaOlustu)
        {
            Destroy(GameObject.FindGameObjectWithTag("NextPiece"));
            RandomBlock = RandomNextBlock;
        }
        RandomNextBlock = Random.Range(0, 7);
        Instantiate(Pieces[RandomBlock], new Vector3(5.5f, 20.5f, 0), Quaternion.identity);
        Instantiate(NextPieces[RandomNextBlock], new Vector3(NextTable.position.x, NextTable.position.y, 1), Quaternion.identity);
    }
    //Pause Button Code
    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
    }
    //Resume and Close Button Code
    public void Resume()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);
    }
    //New Game Button Code
    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameoverMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    //Settings Button code
    public void Settings()
    {
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    //Music Toggle Code
    public void Music()
    {
        if(GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute)
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute = false;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute = true; 
        }
    }
    //Sounds Toggle Code
    public void Sounds()
    {
        if(PieceController.FindObjectOfType<AudioSource>().mute==false)
        {
            PieceController.FindObjectOfType<AudioSource>().mute = true;
            isMuted = true;
            PlayerPrefs.SetInt("Muted", 2);
        }
        else
        {
            PieceController.FindObjectOfType<AudioSource>().mute = false;
            isMuted = false;
            PlayerPrefs.SetInt("Muted", 1);
        }
        
    }
    //Hold Button Code
    public void Hold()
    {
        if (HoldNumber != RandomBlock)
        {
            if (HoldNumber == 9)
            {
                HoldNumber = RandomBlock;
                Destroy(FindObjectOfType<PieceController>().gameObject);
                Instantiate(HoldPieces[HoldNumber], new Vector3(HoldTable.position.x, HoldTable.position.y, 1), Quaternion.identity);
                SpawnNewPiece();
                IlkParcaOlustu = true;
            }
            else
            {
                Destroy(FindObjectOfType<PieceController>().gameObject);
                Instantiate(Pieces[HoldNumber], new Vector3(5.5f, 20.5f, 0), Quaternion.identity);
                Destroy(GameObject.FindGameObjectWithTag("HoldPiece"));
                HoldNumber = RandomBlock;
                Instantiate(HoldPieces[HoldNumber], new Vector3(HoldTable.position.x, HoldTable.position.y, 1), Quaternion.identity);
            }
        }
    }
    void CheckMutedSounds()
    {
        if (PlayerPrefs.GetInt("Muted") == 1)
        {
            isMuted = false;
            SoundToggle.isOn = true;
        }
        else
        {
            isMuted = true;
            SoundToggle.isOn = false;
            Sounds();
        }
    }
    void CheckMutedMusic()
    {
        if(PlayerPrefs.GetInt("Music")==2)
        {
            MusicToggle.isOn = false;
            Music();
        }
        else
        {
            MusicToggle.isOn = true;
        }
    }
    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void MoveLeftFunc()
    {
        MoveLeft = true;
    }
    public void MoveRightFunc()
    {
        MoveRight = true;
    }
    public void MoveDownFunc()
    {
        MoveDown = true;
    }
    public void RotateFunc()
    {
        RotatePiece = true;
    }
}