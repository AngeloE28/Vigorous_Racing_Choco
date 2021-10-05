using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private PlayerInputs carController;

    // Smooth values
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
    }

    private void LateUpdate()
    {
        // Move and rotate the camera pivot to be the same as the player
        transform.position = player.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, smoothVal * Time.deltaTime);
    }
}
