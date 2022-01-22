using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager gameManager;
    private static AudioSource music;

    public static SoundManager Singleton
    {
        get
        {
            if (gameManager == null)
            {
                gameManager = new SoundManager();
                gameManager.Init();
                if (music == null)
                {

                }
            }
            return gameManager;
        }
    }
    private SoundManager()
    {
    }

    private void Init()
    {
        if (music == null)
        {

        }
    }
}
