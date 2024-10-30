using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip WooshSoundClip;
    public AudioClip MetalSoundClip;

    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = WooshSoundClip;
    }

    public void PlayWhoosh()
    {
        _audioSource.pitch = 0.91f;
        _audioSource.clip = WooshSoundClip;
        _audioSource.Play();
    }

    public void PlayMetal()
    {
        _audioSource.pitch = 1f;
        _audioSource.clip = MetalSoundClip;
        _audioSource.Play();
    }
}
