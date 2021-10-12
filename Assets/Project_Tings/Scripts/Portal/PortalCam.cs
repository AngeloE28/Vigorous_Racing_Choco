using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCam : MonoBehaviour
{
    [SerializeField] private Transform playerCam;
    [SerializeField] private Transform portalA;
    [SerializeField] private Transform portalB;
    
    // Update is called once per frame
    void Update()
    {
        // Move the camera around with player position
        Vector3 playerOffsetFromPortal = playerCam.position - portalB.position;
        transform.position = portalA.position + playerOffsetFromPortal;

        // Rotate camera depending on the player's rotation
        float angularDiffBetweenPortalRotations = Quaternion.Angle(portalA.rotation, portalB.rotation);

        Quaternion portalRotationalDiff = Quaternion.AngleAxis(angularDiffBetweenPortalRotations, Vector3.up);

        Vector3 newCamDir = portalRotationalDiff * playerCam.forward;
        transform.rotation = Quaternion.LookRotation(newCamDir, Vector3.up);
    }
}
