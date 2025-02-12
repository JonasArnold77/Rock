using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip WooshSoundClip;
    public AudioClip MetalSoundClip;
    public AudioClip SnareSoundClip;

    public List<AudioClip> ElectricSoundClipList = new List<AudioClip>();

    public AudioSource _audioSource1;
    public AudioSource _audioSource2;

    private void Start()
    {
        _audioSource1.clip = WooshSoundClip;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    PlayMetal();
        //}
    }

    public void PlayWhoosh()
    {
        return;
        _audioSource1.pitch = 0.91f;
        _audioSource1.clip = WooshSoundClip;
        _audioSource1.Play();
    }

    public void PlayMetal()
    {
        return;
        _audioSource2.pitch = 1f;
        _audioSource1.clip = MetalSoundClip;
        _audioSource1.Play();
    }

    public void PlaySnare()
    {
        return;
        _audioSource1.pitch = 1f;
        _audioSource2.clip = MetalSoundClip;
        _audioSource2.Play();
    }

    public void PlayElectricalSound()
    {
        return;
        //_audioSource2.pitch = 1f;
        _audioSource2.clip = ElectricSoundClipList[Random.Range(0, ElectricSoundClipList.Count)];
        _audioSource2.Play();
    }
}
