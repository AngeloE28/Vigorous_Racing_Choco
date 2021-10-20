using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerInputs carController;

    [Header("Camera Positions")]
    [SerializeField] private float originalFOV = 40;
    [SerializeField] private float boostFOV = 60;
    [SerializeField] private Camera playerCam;    
    private bool camBoostPos = false;
    private float camPosIncrement = 0.15f;

    // Smooth values
    [Header("Camera Rotations")]
    private float smoothVal = 3.0f;
    [SerializeField] private float groundSmoothVal = 3.0f;
    [SerializeField] private float driftSmoothVal = 2.5f;
    [SerializeField] private float airSmoothVal = 0.1f;

    [Header("Camera LookAround")]    
    [SerializeField] private float maxHorizontalAngle = 360.0f;
    [SerializeField] private float maxVerticalAngle = 120.0f;

    private void Start()
    {
        smoothVal = groundSmoothVal;
    }

    private void Update()
    {
        // Only follow the rotation of the y axis and ignore the x and z axis
        if (carController.GetIsGroundedState() && carController.GetIsDrifting())
            smoothVal = driftSmoothVal;
        if (carController.GetIsGroundedState() && !carController.GetIsDrifting())
            smoothVal = groundSmoothVal;
        if (!carController.GetIsGroundedState())
            smoothVal = airSmoothVal;
        
        // Move the camera position if the player is boosting or finished boosting
        float boostFOVMultiplier = 5.0f;
        if (camBoostPos)
        {
            if (playerCam.fieldOfView != boostFOV && playerCam.fieldOfView < boostFOV)
                playerCam.fieldOfView += camPosIncrement * boostFOVMultiplier;
        }
        else
        {
            if (playerCam.fieldOfView != originalFOV && playerCam.fieldOfView > originalFOV)
                playerCam.fieldOfView -= camPosIncrement;
        }
        CamLookAround();
    }

    private void LateUpdate()
    {
        // Move and rotate the camera pivot to be the same as the player
        transform.position = player.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, smoothVal * Time.deltaTime);        
    }

    private void CamLookAround()
    {
        // Read the value of each axis
        Vector2 lookAroundVal = carController.GetPlayerInputActions().Player.LookAround.ReadValue<Vector2>();

        // Limit the angle player can look around
        float x_Axis = lookAroundVal.x * maxHorizontalAngle * Time.deltaTime;
        float y_Axis = lookAroundVal.y * maxVerticalAngle * Time.deltaTime;        

        // Apply rotations
        transform.Rotate(Vector3.right * y_Axis);
        transform.Rotate(Vector3.up * x_Axis);
    }

    public void SetCamPos(bool isBoosting)
    {
        // Did the player boost or not?
        camBoostPos = isBoosting;
    }
}
