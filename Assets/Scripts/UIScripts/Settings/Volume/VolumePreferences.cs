using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VolumeSaveData", menuName = "PlayerPrefences/VolumeData", order = 1)]
public class VolumePreferences : ScriptableObject
{
    [Range(0f, 1f)]public float masterVolume;
    [Range(0f, 1f)]public float musicVolume;
    [Range(0f, 1f)]public float ambientVolume;
    [Range(0f, 1f)]public float soundEffectVolume;


    public void ResetPreferences() {
        masterVolume = 0f;
        musicVolume = 0f;
        ambientVolume = 0f;
        soundEffectVolume = 0f;
    }

}
