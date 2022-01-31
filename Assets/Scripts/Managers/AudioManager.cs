using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip tapClip;
    [SerializeField] AudioClip gameOverClip;


    public void PlayGameOverClip()
    {
        audioSource.PlayOneShot(gameOverClip);
    }

    public void PlayTapClip()
    {
        audioSource.PlayOneShot(tapClip);
    }

}
