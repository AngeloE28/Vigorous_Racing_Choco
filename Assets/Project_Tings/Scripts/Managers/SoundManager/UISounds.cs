using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISounds : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioSource uiSFX;
    [SerializeField] private AudioClip uiSelect;
    [SerializeField] private AudioClip uiSwitch;

    
    public void HoverSound()
    {
        uiSFX.PlayOneShot(uiSwitch);
    }

    public void SelectedSound()
    {
        uiSFX.PlayOneShot(uiSelect);
    }
}
