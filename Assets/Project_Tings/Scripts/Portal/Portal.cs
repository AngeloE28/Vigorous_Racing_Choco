using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Portal otherPortal;
    [SerializeField] private MeshRenderer screen;

    // Cameras
    private Camera playerCam;
    private Camera portalCam;
    private RenderTexture viewTexture;

    private void Awake()
    {
        playerCam = Camera.main;
        portalCam = GetComponentInChildren<Camera>();
        portalCam.enabled = false;
    }

    private void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
                viewTexture.Release();

            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal cam to the texture
            portalCam.targetTexture = viewTexture;
            // Display the texture on the other portal
            otherPortal.screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    public void Render()
    {
        if (!VisibleFromCamera(otherPortal.screen, playerCam))
            return; // Player cant see it, don't render

        screen.enabled = false;
        CreateViewTexture();

        // Make portal cam pos and rotation the same relative to this portal as player cam relative to toehr portal
        var mat = transform.localToWorldMatrix * otherPortal.transform.worldToLocalMatrix * playerCam.transform.localToWorldMatrix;
        portalCam.transform.SetPositionAndRotation(mat.GetColumn(3), mat.rotation);

        // Render the camera
        portalCam.Render();

        screen.enabled = true;
    }

    static bool VisibleFromCamera(Renderer renderer, Camera cam)
    {
        // Check if player can see it
        Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(frustrumPlanes, renderer.bounds);
    }
}
