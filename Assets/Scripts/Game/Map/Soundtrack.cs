using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Soundtrack : MonoBehaviour
{
    public AudioClip Clip;
    public bool RunOnStartup;
    public bool Looped;
    [Range(0f, 1f)] public float Volume;
    
    private static AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (RunOnStartup)
        {
            audioSource.loop = Looped;
            audioSource.clip = Clip;
            audioSource.volume = Volume;
            audioSource.Play();
        }
    }

    public static void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public static void Pause()
    {
        audioSource.Pause();
    }

    public static void Play(AudioClip clip, bool looped, float volume = 1f)
    {
        audioSource.loop = looped;
        audioSource.clip = clip;
        audioSource.volume = volume;

        if(PauseMenu.GameIsPaused)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }
}
