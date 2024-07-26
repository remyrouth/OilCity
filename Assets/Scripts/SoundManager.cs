using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private  List<SingleSoundPlayer> soundClipList = new List<SingleSoundPlayer>();

    public void AddSoundScript(SingleSoundPlayer addition) {
        soundClipList.Add(addition);
    }

    public void RemoveSoundScript(SingleSoundPlayer subraction) {
        soundClipList.Remove(subraction);
    }

}
