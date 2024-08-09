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
    private SingleSoundPlayer cameraMusicPlayer = null;
    private int currentMusicTrackIndex = 0;
    [Header("Ambient Track List Variables")]

    [SerializeField]
    private List<MusicTrack> ambientTrackList = new List<MusicTrack>();
    private SingleSoundPlayer cameraAmbiencePlayer = null;
    private int currentAmbienceTrackIndex = 0;

    [Header("City Track List Variables")]
    [SerializeField]
    private List<MusicTrack> citySoundList = new List<MusicTrack>();
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





    // this variable will be used to make tracks
    // started by this manager start sooner
    // mateo noticed there was a blank pause
    // between tracks. this will fix it.
    private float trackLeadTime = 0.5f;



    private  List<SingleSoundPlayer> soundClipList = new List<SingleSoundPlayer>();

    private void Awake()
    {
        AddCameraSoundTrack(citySoundList, cameraMusicPlayer, ref citySoundListIndex, SingleSoundPlayer.SoundType.AmbientSoundEffect);
        AddCameraSoundTrack(ambientTrackList, cameraAmbiencePlayer, ref currentAmbienceTrackIndex, SingleSoundPlayer.SoundType.AmbientSoundEffect);
        AddCameraSoundTrack(musicTrackList, citySoundPlayer, ref currentMusicTrackIndex, SingleSoundPlayer.SoundType.MusicTrack);
        // AddCamerAmbientTrack();
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = false;
        
    }

    private void Start() {
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = true;
    }
    public void AddCameraSoundTrack(List<MusicTrack> trackList, SingleSoundPlayer SFXPlayer, ref int currentTrackIndex, SingleSoundPlayer.SoundType soundType)
    {
        if (currentTrackIndex >= trackList.Count) return;

        if (SFXPlayer == null) {
            SFXPlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();
        }
        SFXPlayer.InitializeFromSoundManager(trackList[currentTrackIndex].musicTrack, soundType);

        if (currentTrackIndex < trackList.Count - 1)
        {
            int index = currentTrackIndex;
            // Invoke("ContinuePlayingTracks", trackList[currentTrackIndex].musicTrack.length - trackLeadTime);
            // this.Invoke(() => AddCameraSoundTrack(trackList, SFXPlayer, currentTrackIndex), trackList[currentTrackIndex].musicTrack.length - trackLeadTime);
            StartCoroutine(ContinuePlayingTracksCoroutine(trackList, SFXPlayer, currentTrackIndex, soundType, trackList[currentTrackIndex].musicTrack.length - trackLeadTime));
        }

        currentTrackIndex++;
    }


    private IEnumerator ContinuePlayingTracksCoroutine(List<MusicTrack> trackList, SingleSoundPlayer SFXPlayer, int currentTrackIndex, SingleSoundPlayer.SoundType soundType, float delay)
    {
        yield return new WaitForSeconds(delay);
        AddCameraSoundTrack(trackList, SFXPlayer, ref currentTrackIndex, soundType);
    }






    

    public void AddSoundScript(SingleSoundPlayer addition) {
        soundClipList.Add(addition);
    }

    public void RemoveSoundScript(SingleSoundPlayer subraction) {
        soundClipList.Remove(subraction);
    }

    public void SelectBuildingSFXTrigger() {
        Debug.Log("SoundEffect Building occured");
        buildingClick.ActivateWithForeignTrigger();
    }

    public void SelectButtonSFXTrigger() {
        buttonClick.ActivateWithForeignTrigger();
    }

    public void SelectBuildingDestroySFXTrigger() {
        destroyBuilding.ActivateWithForeignTrigger();
    }

    public void SelectPipeSFXTrigger() {
        placePipe.ActivateWithForeignTrigger();
    }

    public void PauseContinuousSounds() {
        cameraMusicPlayer.PauseWithForeignTrigger();
        cameraAmbiencePlayer.PauseWithForeignTrigger();
    }

    public void PlayContinuousSounds() {
        cameraMusicPlayer.ActivateWithForeignTrigger();
        cameraAmbiencePlayer.ActivateWithForeignTrigger();
    }

}
