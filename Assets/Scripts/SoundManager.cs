using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Serializable]
    public class MusicTrack {
        public AudioClip musicTrack;
        public int musicTrackoreder;
    }


    [Header("Music Track List Variables")]
    public List<MusicTrack> musicTrackList = new List<MusicTrack>();
    private SingleSoundPlayer cameraMusicPlayer = null;
    private int currentTrackIndex = 0;



    private  List<SingleSoundPlayer> soundClipList = new List<SingleSoundPlayer>();

    private void Awake()
    {
        AddCameraMusicTrack();
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = false;
        
    }

    private void Start() {
        Camera.main.gameObject.GetComponent<AudioSource>().enabled = true;
    }

    private void AddCameraMusicTrack() {
        if (currentTrackIndex >= musicTrackList.Count) return; // dev bugfix

        if (cameraMusicPlayer != null) {
            cameraMusicPlayer.enabled = false;
            Destroy(cameraMusicPlayer);
            cameraMusicPlayer = null;
        }
        cameraMusicPlayer = Camera.main.gameObject.AddComponent<SingleSoundPlayer>();

        cameraMusicPlayer.InitializeFromSoundManager(musicTrackList[currentTrackIndex].musicTrack);
        if (currentTrackIndex < musicTrackList.Count - 1) {
            Invoke("AddCameraMusicTrack", musicTrackList[currentTrackIndex].musicTrack.length);
            currentTrackIndex++;
        }


    }

    public void AddSoundScript(SingleSoundPlayer addition) {
        soundClipList.Add(addition);
    }

    public void RemoveSoundScript(SingleSoundPlayer subraction) {
        soundClipList.Remove(subraction);
    }

}
