using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{

    [SerializeField] AudioMixer mixer;

    private void Awake()
    {
        SetUpSingleton();
    }

    private void SetUpSingleton()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }


    public void SoundButton()
    {
        if (mixer.GetFloat("SoundVolume", out float value))
        {
            if (value == -80f)
            {
                mixer.SetFloat("SoundVolume", 0f);
                return;
            }
            if (value != -80f)
            {
                mixer.SetFloat("SoundVolume", -80f);
                return;
            }
        }
    }

    public void MusicButton()
    {
        if (mixer.GetFloat("MusicVolume", out float value))
        {
            if (value == -80f)
            {
                mixer.SetFloat("MusicVolume", 0f);
                return;
            }
            if (value != -80f)
            {
                mixer.SetFloat("MusicVolume", -80f);
                return;
            }
        }
    }

}
