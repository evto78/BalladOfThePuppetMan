using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundOnHoverClick : MonoBehaviour
{
    [Header("On Hover")]
    public int soundIDHover;
    public AudioManager.SFXType soundTypeHover;
    public float soundVolHover;
    public float soundPitchHover;

    [Header("On Click")]
    public int soundIDClick;
    public AudioManager.SFXType soundTypeClick;
    public float soundVolClick;
    public float soundPitchClick;

    AudioManager audioMan;
    Button button;
    ReadoutOnHover readout;
    void Start()
    {
        audioMan = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        TryGetComponent<Button>(out button);
        TryGetComponent<ReadoutOnHover>(out readout);
    }
}
