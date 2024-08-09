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
    [SerializeField]
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

    public void InitializeFromSoundManager(AudioClip musicTrack, SoundType enumSoundType) {
        // print prior sound type?
        soundType = enumSoundType;
        settingsManager.OnSoundEffectVolumeChanged -= ChangePercentOutput;
        switch (soundType)
        {
            case SoundType.AmbientSoundEffect:
                settingsManager.OnAmbientSoundVolumeChanged += ChangePercentOutput;
                break;
            case SoundType.MusicTrack:
                settingsManager.OnMusicVolumeChanged += ChangePercentOutput;
                break;
            default:
                break;
        }
        SettingsManager.Instance.VolumeInitializationForSoundPlayers();
        shouldLoop = false;
        maxVolume = 1f;
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
