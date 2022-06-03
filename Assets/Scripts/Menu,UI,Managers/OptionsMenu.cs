using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private float masterVolume;
    private float musicVolume;
    private float SFXVolume;

    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = _slider.value;
        if(whatValue == "Master")
        {
            masterVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume); 
        }
        if(whatValue == "Music")
        {
            musicVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume); 
        }
        if(whatValue == "Sounds")
        {
            SFXVolume = _slider.value;
            AkSoundEngine.SetRTPCValue("SFXVolume", SFXVolume); 
        }
    }
}
