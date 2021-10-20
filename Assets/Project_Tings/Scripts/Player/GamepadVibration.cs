using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadVibration : MonoBehaviour
{
    [Header("Gamepad Vibration Stats")]    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float leftFreqVal = 0.35f;
    [Range(0.0f, 1.0f)]
    [SerializeField] private float rightFreqVal = 0.35f;    
    [SerializeField] private float updateSpeedFreq = 0.25f;
    private float zeroFreqValX = 0.0f, zeroFreqValY = 0.0f;    
    private Gamepad gamepad;
    private bool shakeGamepad = false;

    private void Awake()
    {
        gamepad = Gamepad.current;
    }

    private void Start()
    {        
        InvokeRepeating(nameof(UpdateMotorSpeed), 0.0f, updateSpeedFreq);
    }

    // Update is called once per frame
    void UpdateMotorSpeed()
    {
        if (gamepad != null)
        {
            if (shakeGamepad)
                gamepad.SetMotorSpeeds(leftFreqVal, rightFreqVal);
            else
                gamepad.SetMotorSpeeds(zeroFreqValX, zeroFreqValY);
        }
    }

    public void SetShakeGamepadState(bool shake)
    {
        shakeGamepad = shake;
    }
}
