using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float airSmoothVal = 0.1f;

    private void Update()
    {
        // Only follow the rotation of the y axis and ignore the x and z axis
        if (carController.GetIsGroundedState())
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
    }

    private void LateUpdate()
    {
        // Move and rotate the camera pivot to be the same as the player
        transform.position = player.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, smoothVal * Time.deltaTime);
    }

    public void SetCamPos(bool boost)
    {
        // Did the player boost or not?
        camBoostPos = boost;
    }
}
