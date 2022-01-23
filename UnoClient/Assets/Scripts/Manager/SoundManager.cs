using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    Welcome = 1,
    Play = 2,
    Win = 3,
    Lose = 4,
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager gameManager;
    private static bool inited = false;
    private AudioSource music;
    
    private Dictionary<Music, string> MUSIC_PATH = new Dictionary<Music, string>()
    {
        {Music.Welcome, "Sound/MusicEx_Welcome" },
        {Music.Play, "Sound/MusicEx_Normal" },
        {Music.Win, "Sound/MusicEx_Win" },
        {Music.Lose, "Sound/MusicEx_Lose" },
    };

    public static SoundManager Singleton
    {
        get
        {
            if (gameManager == null && !inited)
            {
                gameManager = new SoundManager();
                gameManager.Init();
            }
            return gameManager;
        }
    }
    private SoundManager()
    {
    }

    public void Init()
    {
        if (!inited)
        {
            inited = true;
            var obj = new GameObject("[music]");
            DontDestroyOnLoad(obj);
            music = obj.AddComponent<AudioSource>();
            music.loop = true;
        }
    }

    public void PlayMusic(Music musicName)
    {
        if(music != null)
        {
            if (music.isPlaying)
            {
                music.Stop();
            }
            AudioClip audioClip = Resources.Load(MUSIC_PATH[musicName], typeof(AudioClip)) as AudioClip;
            music.clip = audioClip;
            music.Play();
        }
        else
        {
            Debug.LogError("!!!!!");
        }
    }
}

