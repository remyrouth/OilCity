using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Managers;

public class SingleSoundPlayer : MonoBehaviour
{
    [SerializeField] private bool shouldLoop;
    [SerializeField] [Range(0, 1)] private float maxVolume = 1f;
    [SerializeField] private AudioClip soundClip;
    [SerializeField] private SoundType soundType = SoundType.SoundEffect;
    [SerializeField] private bool usesForeignTrigger;

    private AudioSource audioSource;
    
    private void Awake()
    {
        if (audioSource is null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        switch (soundType)
        {
            case SoundType.SoundEffect:
                // subscribe to soundEffectVolume volume changes from settingsManager script
                SettingsManager.Instance.OnSoundEffectVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.AmbientSoundEffect:
                // subscribe to ambientSoundVolume volume changes from settingsManager script
                SettingsManager.Instance.OnAmbientSoundVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                // subscribe to musicVolume volume changes from settingsManager script
                SettingsManager.Instance.OnMusicVolumeChanged += ChangePercentOutput;
                break;
        }
    }

    private void Start()
    {
        SoundManager.Instance.AddSoundScript(this);

        audioSource.volume = soundType switch
        {
            SoundType.SoundEffect => Math.Min(SettingsManager.Instance.MasterVolume * 
                                              SettingsManager.Instance.SoundEffectVolume, maxVolume),
            SoundType.AmbientSoundEffect => Math.Min(SettingsManager.Instance.MasterVolume * 
                                                     SettingsManager.Instance.AmbientSoundVolume, maxVolume),
            SoundType.MusicTrack => Math.Min(SettingsManager.Instance.MasterVolume * 
                                             SettingsManager.Instance.MusicVolume, maxVolume),
            _ => throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null)
        };
        
        audioSource.loop = shouldLoop;
        audioSource.clip = soundClip;
        if (!usesForeignTrigger) {
            audioSource.Play();
        }
    }

    public void ActivateWithForeignTrigger() {
        // Debug.Log("ActivateWithForeignTrigger method happened");
        audioSource.Play();
    }

    public void PauseWithForeignTrigger() {
        // Debug.Log("PauseWithForeignTrigger method happened");
        audioSource.Pause();
    }

    public float GetSoundEffectLength() {
        return audioSource.clip.length;
    }

    public void InitializeFromSoundManager(AudioClip musicTrack, float newMaxVolume, SoundType enumSoundType) {
        // print prior sound type?
        soundType = enumSoundType;
        SettingsManager.Instance.OnSoundEffectVolumeChanged -= ChangePercentOutput;
        switch (soundType)
        {
            case SoundType.AmbientSoundEffect:
                SettingsManager.Instance.OnAmbientSoundVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                SettingsManager.Instance.OnMusicVolumeChanged += ChangePercentOutput;
                break;
        }
        //SettingsManager.Instance.VolumeInitializationForSoundPlayers();
        shouldLoop = false;
        maxVolume = newMaxVolume;
        soundClip = musicTrack;
        usesForeignTrigger = true;
        if (audioSource != null) {
            audioSource.clip = soundClip;
            audioSource.volume = 1f;
        }

        audioSource.Play();
    }

    // This method changes the percentage of the total volume
    // we are allowed to output
    private void ChangePercentOutput(float newPercent)
    {
        audioSource.volume = maxVolume * newPercent;
    }

    private void OnDisable()
    {
        SoundManager.Instance.RemoveSoundScript(this);
        Destroy(audioSource);

        switch (soundType)
        {
            case SoundType.SoundEffect:
                // cancel subscription to soundEffectVolume volume changes from settingsManager script
                SettingsManager.Instance.OnSoundEffectVolumeChanged -= ChangePercentOutput;
                break;
            case SoundType.AmbientSoundEffect:
                // cancel subscription to ambientSoundVolume volume changes from settingsManager script
                SettingsManager.Instance.OnAmbientSoundVolumeChanged -= ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                // cancel subscription to musicVolume volume changes from settingsManager script
                SettingsManager.Instance.OnMusicVolumeChanged -= ChangePercentOutput;
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
