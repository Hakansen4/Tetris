using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicController : MonoBehaviour
{
    AudioSource _audiosrc;
    GameObject [] anyMusic;
    bool haveCreated = false;
    void Awake()
    {
        anyMusic = GameObject.FindGameObjectsWithTag("Music");
        if (anyMusic.Length<2)
            haveCreated = false;
        else
            haveCreated = true;
        if (haveCreated)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
        _audiosrc = GetComponent<AudioSource>();
    }
    public void Play()
    {
        if (_audiosrc.isPlaying) return;
        _audiosrc.Play();
    }
    public void Stop()
    {
        if (!_audiosrc.isPlaying) return;
        _audiosrc.Stop();
    }
}
