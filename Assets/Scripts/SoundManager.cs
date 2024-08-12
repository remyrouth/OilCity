using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [Serializable]
    public class MusicTrack {
        public AudioClip musicTrack;
        public int musicTrackoreder;
    }


    [Header("Music Track List Variables")]

    [SerializeField]
    private List<MusicTrack> musicTrackList = new List<MusicTrack>();
    [SerializeField]
    private float musicMaxVol = 1f;
    private SingleSoundPlayer cameraMusicPlayer = null;
    private int currentMusicTrackIndex = 0;

    [Header("Ambient Track List Variables")]

    [SerializeField]
    private List<MusicTrack> ambientTrackList = new List<MusicTrack>();
    [SerializeField]
    private float ambientMaxVol = 1f;
    private SingleSoundPlayer cameraAmbiencePlayer = null;
    private int currentAmbienceTrackIndex = 0;

    [Header("City Track List Variables")]
    [SerializeField]
    private List<MusicTrack> citySoundList = new List<MusicTrack>();
    [SerializeField]
    private float cityMaxVol = 1f;
    private SingleSoundPlayer citySoundPlayer;
    private int citySoundListIndex = 0;
    
    [Header("UI Sound Effect Variables")]
    [SerializeField]
    private SingleSoundPlayer buildingClick;
    [SerializeField]
    private SingleSoundPlayer buttonClick;

    [Header("AfterEffects Sound Variables")]
    [SerializeField]
    private SingleSoundPlayer destroyBuilding;

    [SerializeField]
    private SingleSoundPlayer placePipe;
    [SerializeField]
    private bool canSelfActivateMusic = false;
    // this variable will be used to make tracks
    // started by this manager start sooner
    // mateo noticed there was a blank pause
    // between tracks. this will fix it.
    private float trackLeadTime = 0f;
    
    private  List<SingleSoundPlayer> soundClipList = new List<SingleSoundPlayer>();

    private void Awake()
    {

        if(Instance != null) {
            if(Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        // if (SFXPlayer == null) {
        //     SFXPlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();
        // }
        cameraMusicPlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();
        cameraAmbiencePlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();
        citySoundPlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();



        AddCameraSoundTrack(citySoundList, citySoundPlayer, ref citySoundListIndex, cityMaxVol, SingleSoundPlayer.SoundType.AmbientSoundEffect);
        AddCameraSoundTrack(ambientTrackList, cameraAmbiencePlayer, ref currentAmbienceTrackIndex, ambientMaxVol, SingleSoundPlayer.SoundType.AmbientSoundEffect);
        if (canSelfActivateMusic) {
            AddCameraSoundTrack(musicTrackList, cameraMusicPlayer, ref currentMusicTrackIndex, musicMaxVol, SingleSoundPlayer.SoundType.MusicTrack);
        }
        // AddCameraSoundTrack(musicTrackList, cameraMusicPlayer, ref currentMusicTrackIndex, musicMaxVol, SingleSoundPlayer.SoundType.MusicTrack);
        // AddCamerAmbientTrack();
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = false;
        
    }

    private void Start() {
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = true;
    }
    public void AddCameraSoundTrack(List<MusicTrack> trackList, SingleSoundPlayer SFXPlayer, ref int currentTrackIndex, float maxVol, SingleSoundPlayer.SoundType soundType)
    {
        try {
            if (currentTrackIndex >= trackList.Count) return;



            if (SFXPlayer == cameraAmbiencePlayer) {
                Debug.Log("Ambience Length: " + trackList[currentTrackIndex].musicTrack.length);
            }

            SFXPlayer.InitializeFromSoundManager(trackList[currentTrackIndex].musicTrack, maxVol, soundType);


            int index = currentTrackIndex;
            // Invoke("ContinuePlayingTracks", trackList[currentTrackIndex].musicTrack.length - trackLeadTime);
            // this.Invoke(() => AddCameraSoundTrack(trackList, SFXPlayer, currentTrackIndex), trackList[currentTrackIndex].musicTrack.length - trackLeadTime);
            currentTrackIndex++;
            StartCoroutine(ContinuePlayingTracksCoroutine(trackList, SFXPlayer, currentTrackIndex, maxVol, soundType, trackList[index].musicTrack.length - trackLeadTime));

            // currentTrackIndex++;
        } catch (Exception error) {

        }

    }
    
    private IEnumerator ContinuePlayingTracksCoroutine(List<MusicTrack> trackList, SingleSoundPlayer SFXPlayer, int currentTrackIndex, float newMaxVolume, SingleSoundPlayer.SoundType soundType, float delay)
    {
        if (SFXPlayer == cameraAmbiencePlayer) {
            Debug.Log("Delay Started");
            Debug.Log("Ambience Length: " + trackList[currentTrackIndex].musicTrack.length);
        }
        yield return new WaitForSeconds(delay);
        AddCameraSoundTrack(trackList, SFXPlayer, ref currentTrackIndex, newMaxVolume, soundType);
    }

    public void BeginMusicTrackFromTutorial() {
        AddCameraSoundTrack(musicTrackList, cameraMusicPlayer, ref currentMusicTrackIndex, musicMaxVol, SingleSoundPlayer.SoundType.MusicTrack);
    }
    
    public void AddSoundScript(SingleSoundPlayer addition) {
        soundClipList.Add(addition);
    }

    public void RemoveSoundScript(SingleSoundPlayer subraction) {
        soundClipList.Remove(subraction);
    }

    public void SelectBuildingSFXTrigger() {
        if (buildingClick) {
            buildingClick.ActivateWithForeignTrigger();
        }
    }

    public void SelectButtonSFXTrigger() {
        if (buttonClick) {
            buttonClick.ActivateWithForeignTrigger();
        }
    }

    public void SelectBuildingDestroySFXTrigger() {
        if (destroyBuilding) {
            destroyBuilding.ActivateWithForeignTrigger();
        }
    }

    public void SelectPipeSFXTrigger() {
        if (placePipe) {
            placePipe.ActivateWithForeignTrigger();
        }
    }

    public void PauseContinuousSounds() {
        if (cameraMusicPlayer && cameraAmbiencePlayer) {
            cameraMusicPlayer.PauseWithForeignTrigger();
            cameraAmbiencePlayer.PauseWithForeignTrigger();
        }
    }

    public void PlayContinuousSounds() {
        if (cameraMusicPlayer) {
            cameraMusicPlayer.ActivateWithForeignTrigger();
        }

        if (cameraAmbiencePlayer) {
            cameraAmbiencePlayer.ActivateWithForeignTrigger();
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }

}
