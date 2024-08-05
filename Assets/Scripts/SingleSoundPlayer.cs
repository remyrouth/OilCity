using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Managers;

public class SingleSoundPlayer : Singleton<SingleSoundPlayer>
{
    [SerializeField]
    private bool shouldLoop = false;
    [SerializeField]
    [Range(0, 1)]
    private float maxVolume = 1f;
    [SerializeField]
    private AudioClip soundClip;
    [SerializeField]
    private SoundType soundType = SoundType.SoundEffect;

    private bool usesForeignTrigger = false;

    private AudioSource audioSource;
    private SettingsManager settingsManager;

    private void OnEnable()
    {
        FindObjectOfType<SoundManager>().AddSoundScript(this);
        settingsManager = FindObjectOfType<SettingsManager>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = maxVolume;
        audioSource.loop = shouldLoop;
        audioSource.clip = soundClip;
        

        switch (soundType)
        {
            case SoundType.SoundEffect:
                // subscribe to soundEffectVolume volume changes from settingsManager script
                settingsManager.OnSoundEffectVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.AmbientSoundEffect:
                // subscribe to ambientSoundVolume volume changes from settingsManager script
                settingsManager.OnAmbientSoundVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                // subscribe to musicVolume volume changes from settingsManager script
                settingsManager.OnMusicVolumeChanged += ChangePercentOutput;
                break;
            default:
                break;
        }

        audioSource.enabled = false;
        audioSource.enabled = true;

        if (!usesForeignTrigger) {
            audioSource.Play();
        }
    }

    public void ActivateWithForeignTrigger() {
        Debug.Log("ActivateWithForeignTrigger method happened");
        audioSource.Play();
    }

    public float GetSoundEffectLength() {
        return audioSource.clip.length;
    }

    public void InitializeFromSoundManager(AudioClip musicTrack) {
        soundType = SoundType.MusicTrack;
        shouldLoop = false;
        maxVolume = 1f;
        soundClip = musicTrack;
        if (audioSource != null) {
            audioSource.clip = soundClip;
        }
    }

    // This method changes the percentage of the total volume
    // we are allowed to output
    private void ChangePercentOutput(float newPercent)
    {
        audioSource.volume = maxVolume * newPercent;
    }

    private void OnDisable()
    {
        FindObjectOfType<SoundManager>().RemoveSoundScript(this);
        Destroy(audioSource);

        switch (soundType)
        {
            case SoundType.SoundEffect:
                // cancel subscription to soundEffectVolume volume changes from settingsManager script
                settingsManager.OnSoundEffectVolumeChanged -= ChangePercentOutput;
                break;
            case SoundType.AmbientSoundEffect:
                // cancel subscription to ambientSoundVolume volume changes from settingsManager script
                settingsManager.OnAmbientSoundVolumeChanged -= ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                // cancel subscription to musicVolume volume changes from settingsManager script
                settingsManager.OnMusicVolumeChanged -= ChangePercentOutput;
                break;
            default:
                break;
        }
    }

    public enum SoundType
    {
        SoundEffect,
        AmbientSoundEffect,
        MusicTrack
    }
}
