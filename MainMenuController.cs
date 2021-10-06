using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public GameObject Stars1, Stars2;
    float RePositionY = 20.95f, randomPositionX = 0;
    bool isMuted = false;
    int RandomNum = 0;
    public GameObject[] Blocks;public GameObject MainMenu; public GameObject PlayMenu;public GameObject OptionsMenu;
    bool Created = false;
    public TextMeshProUGUI PersonalBestTimer, PersonalBestLevel;
    public Toggle SoundsToggle, MusicToggle;
    void Start()
    {
        CheckMutedMusic();
        CheckMutedSounds();
        PersonalBestLevel.text = "Personal Best " + PlayerPrefs.GetInt(GameController._HighScoreLvl).ToString();
        PersonalBestTimer.text = "Personal Best " + PlayerPrefs.GetInt(GameController._HighScoreTimer).ToString();
    }
    void Update()
    {
        MoveStars();
        RePositionStars();
        if(!Created)
        StartCoroutine(SpawnNewBlock());
    }
    void MoveStars()
    {
        Stars1.transform.position -= new Vector3(0, 1f, 1) * Time.deltaTime;
        Stars2.transform.position -= new Vector3(0, 1f, 1) * Time.deltaTime;
    }
    void RePositionStars()
    {
        if(Stars1.transform.position.y<=0.2f)
        {
            Stars1.transform.position = new Vector2(Stars1.transform.position.x, RePositionY);
        }
        else if (Stars2.transform.position.y <= 0.2f)
        {
            Stars2.transform.position = new Vector2(Stars1.transform.position.x, RePositionY);
        }
    }
    IEnumerator SpawnNewBlock()
    {
        Created = true;
        yield return new WaitForSeconds(6);
        RandomNum = Random.Range(0, 7);
        randomPositionX = Random.Range(-2.3f, 2.4f);
        Instantiate(Blocks[RandomNum], new Vector3(randomPositionX,16.5f,1), Quaternion.identity);
        Created = false;
    }
    public void PlayButton()
    {
        MainMenu.SetActive(false);
        PlayMenu.SetActive(true);
    }
    public void OptionsButton()
    {
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
    }
    public void QuitButton()
    {
        PlayerPrefs.SetInt("Muted", 1);
        PlayerPrefs.SetInt("Music", 1);
        Application.Quit();
    }
    public void PlayTimerModeButton()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void PlayLevelModeButton()
    {
        SceneManager.LoadScene("Level");
    }
    public void Sounds()
    {
        if(isMuted)
        {
            isMuted = false;
            PlayerPrefs.SetInt("Muted", 1);
        }
        else
        {
            isMuted = true;
            PlayerPrefs.SetInt("Muted", 2);
        }
    }
    public void Music()
    {
        if (GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute)
        {
            PlayerPrefs.SetInt("Music", 1);
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute = false;
        }
        else
        {
            PlayerPrefs.SetInt("Music", 2);
            GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>().mute = true;
        } 
    }
    public void BackMenu()
    {
        PlayMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }
    void CheckMutedSounds()
    {
        if (PlayerPrefs.GetInt("Muted") == 1)
        {
            isMuted = false;
            SoundsToggle.isOn = true;
        }
        else
        {
            isMuted = true;
            SoundsToggle.isOn = false;
            Sounds();
        }
    }
    void CheckMutedMusic()
    {
        if (PlayerPrefs.GetInt("Music")==1)
        {
            MusicToggle.isOn = true;
        }
        else
        {
            MusicToggle.isOn = false;
            Music();
        }
    }
}