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

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (RunOnStartup)
        {
            audioSource.loop = Looped;
            audioSource.clip = Clip;
            audioSource.volume = Volume;
            audioSource.Play();
        }
        
        GameState.OnGameStateChange += delegate(eGameState state)
        {
            switch (state)
            {
                case eGameState.PAUSED:
                    Pause();
                    break;
                case eGameState.RUNNING:
                    audioSource.Play();
                    break;
            }
        };
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
        audioSource.Play();
    }
}
