using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Music SoundTrackList")]
    public List<MusicTrack> MusicTrackList = new List<MusicTrack>();
    private int currentTrackIndex = 0;



    private Camera mainCamera;
    private float volumePercentage = 1f;            // on a range of 0-1 percent
    private List<SoundClass> soundClassList = new List<SoundClass>();
    public float soundHearingDistance = 10f;        // The max distance that a sound can be heard from

    private void Start() {
        mainCamera = Camera.main;
        Invoke("StartMusicTrack", 0f);
    }
    private void Update() {
        UpdateSoundVolumes();
        DeleteExcessSoundClasses();
    }

    private void StartMusicTrack() {
        AudioClip soundClip = MusicTrackList[currentTrackIndex].musicClip;
        AudioSource audioSource = mainCamera.gameObject.AddComponent<AudioSource>();
        float volume = MusicTrackList[currentTrackIndex].musicVolume;
        SoundClass newSoundClass = new SoundClass(mainCamera.gameObject, false, soundClip.length, audioSource, volume);
        soundClassList.Add(newSoundClass);

        currentTrackIndex++;
        if (currentTrackIndex > MusicTrackList.Count - 1) {
            currentTrackIndex = 0;
        }
        Invoke("StartMusicTrack", soundClip.length);
    }

    public void AddSound(GameObject sourceObject, AudioClip soundClip,  bool shouldLoop, float soundVolume) {
        AudioSource audioSource = mainCamera.gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        SoundClass newSoundClass = new SoundClass(sourceObject, shouldLoop, soundClip.length, audioSource, soundVolume);
        soundClassList.Add(newSoundClass);
    }

    private void UpdateSoundVolumes() {
        foreach (SoundClass soundClass in soundClassList) {
            soundClass.UpdateVolume(soundHearingDistance);
        }
    }

    private void DeleteExcessSoundClasses() {
        List<SoundClass> deletionList = new List<SoundClass>();

        // lowers the count down timer on each class,
        // collects invalid classes
        foreach (SoundClass soundclass in soundClassList) {
            if (!soundclass.ThisClassLoops()) {
                soundclass.ContinueDeletionCountDown(Time.deltaTime);
                if (soundclass.ShouldBeDeleted()) {
                    deletionList.Add(soundclass);
                }
            }
        }

        // finally deletes invalid classes
        foreach(SoundClass invalidSoundClass in deletionList) {
            invalidSoundClass.KillClass();
            soundClassList.Remove(invalidSoundClass);
        }
    }

    // Simply a data storage 
    [Serializable]
    public class MusicTrack {
        public AudioClip musicClip;
        public float musicVolume;
    }

    // Used to keep track of sounds, and change 
    // their behavior based on said info
    class SoundClass {
        private GameObject referenceObject;
        private AudioClip audioClip;
        private bool shouldLoop;
        private float soundClipLength;
        private AudioSource audioSource;
        private AudioListener listener;
        private float initialVolume;

        public SoundClass(GameObject referenceObject, bool shouldLoop, float soundClipLength, AudioSource audioSource, float initialVolume) 
        {
            this.referenceObject = referenceObject;
            this.shouldLoop = shouldLoop;
            this.soundClipLength = soundClipLength;
            this.audioSource = audioSource;
            audioSource.volume = initialVolume;
            this.listener = FindObjectOfType<AudioListener>();
            this.initialVolume = initialVolume;
        }

        public void UpdateVolume(float maxHearingDistance) {
            Vector2 sourcePos = new Vector2(referenceObject.transform.position.x, referenceObject.transform.position.y);
            Vector2 targetPos = new Vector2(listener.gameObject.transform.position.x, listener.gameObject.transform.position.y);
            float currentDistance = Vector2.Distance(sourcePos, targetPos);
            audioSource.volume = initialVolume * (currentDistance / maxHearingDistance);
        }

        public bool ThisClassLoops() {
            return shouldLoop;
        }

        // If this sound does not loop, we should keep track of when it finishes
        // so that this class can be removed. We'll keep deleting from the 
        // remaining sound clip length till it gets to zero
        public void ContinueDeletionCountDown(float subtractableInput) {
            soundClipLength -= subtractableInput;
        }

        // In the case that we run out of time and soundClipLength is below zero,
        // this method wil let us know we should kill/destroy this class
        public bool ShouldBeDeleted() {
            return soundClipLength < 0;
        }

        // deletes the corrosponding audiosource when we need to kill this class
        public void KillClass() {
            Destroy(audioSource);
        }
        
    }
}
