using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public enum Options
{
    _MAINVOL,
    _SFXVOL,
    _MUSICVOL,
    _UIVOL
}

public enum Sounds
{
    mainVol,
    musicVol,
    sfxVol,
    uiVol
}

public class OptionsManager : MonoBehaviour
{

    [Header("Sounds")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Master")]
    [SerializeField] private Slider mainSlider;
    [SerializeField] private TMP_Text mainVolText;

    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TMP_Text musicVolText;

    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text sfxVolText;
    
    [Header("UI")]
    [SerializeField] private Slider uiSlider;
    [SerializeField] private TMP_Text uiVolText;
    
    // Sound vals
    private float defaultVal = 0.0f;
    private float lowestVal = -50.0f;

    private void Awake()
    {
        defaultVal = 0.0f;
        lowestVal = -50.0f;

        // Check each sound        
        if (!PlayerPrefs.HasKey(Options._MAINVOL.ToString()))
            DefaultVol(Options._MAINVOL, Sounds.mainVol, mainSlider, mainVolText);
        else
            LoadVol(Options._MAINVOL, Sounds.mainVol, mainSlider, mainVolText);

        if (!PlayerPrefs.HasKey(Options._MUSICVOL.ToString()))
            DefaultVol(Options._MUSICVOL, Sounds.musicVol, musicSlider, musicVolText);
        else
            LoadVol(Options._MUSICVOL, Sounds.musicVol, musicSlider, musicVolText);

        if (!PlayerPrefs.HasKey(Options._SFXVOL.ToString()))
            DefaultVol(Options._SFXVOL, Sounds.sfxVol, sfxSlider, sfxVolText);
        else
            LoadVol(Options._SFXVOL, Sounds.sfxVol, sfxSlider, sfxVolText);

        if (!PlayerPrefs.HasKey(Options._UIVOL.ToString()))
            DefaultVol(Options._UIVOL, Sounds.uiVol, uiSlider, uiVolText);
        else
            LoadVol(Options._UIVOL, Sounds.uiVol, uiSlider, uiVolText);
    }

    private void DefaultVol(Options opt, Sounds sound, Slider slider, TMP_Text text)
    {
        PlayerPrefs.SetFloat(opt.ToString(), defaultVal);
        audioMixer.SetFloat(sound.ToString(), defaultVal);

        slider.value = default;
        float newVol = Remap(slider.value, lowestVal, 0.0f, 0.0f, 1.0f);
        text.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

    }

    private void LoadVol(Options opt, Sounds sound, Slider slider, TMP_Text text)
    {
        float vol = PlayerPrefs.GetFloat(opt.ToString());

        slider.value = vol;
        float newVol = Remap(slider.value, lowestVal, 0.0f, 0.0f, 1.0f);
        text.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

        audioMixer.SetFloat(sound.ToString(), vol);
    }

    private void SaveVol(Options opt, Sounds sound, float volume)
    {
        PlayerPrefs.SetFloat(opt.ToString(), volume);
        audioMixer.SetFloat(sound.ToString(), volume);
    }

    public void ChangeMainVol()
    {
        float vol = mainSlider.value;        
        float newVol = Remap(vol, lowestVal, 0.0f, 0.0f, 1.0f);        
        mainVolText.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

        SaveVol(Options._MAINVOL, Sounds.mainVol, vol);
    }

    public void ChangeMusicVol()
    {
        float vol = musicSlider.value;
        float newVol = Remap(vol, lowestVal, 0.0f, 0.0f, 1.0f);
        musicVolText.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

        SaveVol(Options._MUSICVOL, Sounds.musicVol, vol);
    }

    public void ChangeSfxVol()
    {
        float vol = sfxSlider.value;
        float newVol = Remap(vol, lowestVal, 0.0f, 0.0f, 1.0f);
        sfxVolText.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

        SaveVol(Options._SFXVOL, Sounds.sfxVol, vol);
    }

    public void ChangeUIVol()
    {
        float vol = uiSlider.value;
        float newVol = Remap(vol, lowestVal, 0.0f, 0.0f, 1.0f);
        uiVolText.text = Mathf.RoundToInt(newVol * 100.0f) + "%";

        SaveVol(Options._UIVOL, Sounds.uiVol, vol);
    }

    public float Remap(float input, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Solution by kru, (2013)
        // https://forum.unity.com/threads/re-map-a-number-from-one-range-to-another.119437/
        float normal = Mathf.InverseLerp(fromMin, fromMax, input);
        float newVal = Mathf.Lerp(toMin, toMax, normal);

        return newVal;
    }
}
