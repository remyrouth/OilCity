using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private float volumePercentage = 1f;            // on a range of 0-1 percent
    private List<SoundClass> soundClassList = new List<SoundClass>();
    private void Update() {
        UpdateSoundVolumes();
    }

    private void UpdateSoundVolumes() {
        // foreach ()
    }


    class SoundClass {

    }
}
