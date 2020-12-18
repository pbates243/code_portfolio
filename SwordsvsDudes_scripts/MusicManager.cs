using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public float GlobalVolume = 0.25f;

    private void Awake ()
    {
        AudioListener.volume = GlobalVolume;
    }

}
