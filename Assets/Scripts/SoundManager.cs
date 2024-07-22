using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Music SoundTrackList")]


    private Camera mainCamera;
    private float volumePercentage = 1f;            // on a range of 0-1 percent
    private List<SoundClass> soundClassList = new List<SoundClass>();
    public float soundHearingDistance = 10f;        // The max distance that a sound can be heard from

    private void Start() {
        mainCamera = Camera.main;
    }
    private void Update() {
        UpdateSoundVolumes();
        DeleteExcessSoundClasses();
    }

    public void AddSound(GameObject sourceObject, AudioClip soundClip,  bool shouldLoop, float soundVolume) {
        AudioSource audioSource = mainCamera.gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        SoundClass newSoundClass = new SoundClass(sourceObject, shouldLoop, soundClip.length, audioSource, soundVolume);
    }

    private void UpdateSoundVolumes() {
        foreach (SoundClass soundClass in soundClassList) {
            soundClass.UpdateVolume(soundHearingDistance);
        }
    }

    private void DeleteExcessSoundClasses() {
        List<SoundClass> deletionList = new List<SoundClass>();

        // lowers the count down timer on each class
        foreach (SoundClass soundclass in soundClassList) {
            if (!soundclass.ThisClassLoops()) {
                soundclass.ContinueDeletionCountDown(Time.deltaTime);
                if (soundclass.ShouldBeDeleted()) {
                    deletionList.Add(soundclass);
                }
            }
        }

        foreach(SoundClass invalidSoundClass in deletionList) {
            invalidSoundClass.KillClass();
            soundClassList.Remove(invalidSoundClass);
        }
    }


    // class 

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
