using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    [SerializeField] private Camera playerCam;

    [Header("Portal A")]
    [SerializeField] private Camera cameraA;
    [SerializeField] private Material cameraMatA;
    [SerializeField] private GameObject portalARenderFX;
    private Renderer portalARenderer;

    [Header("Portal B")]
    [SerializeField] private Camera cameraB;
    [SerializeField] private Material cameraMatB;
    [SerializeField] private GameObject portalBRenderFX;
    private Renderer portalBRenderer;

    private void Awake()
    {
        playerCam = Camera.main;
        portalARenderer = portalARenderFX.GetComponent<Renderer>();
        portalBRenderer = portalBRenderFX.GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cameraA.targetTexture != null)
            cameraA.targetTexture.Release();

        cameraA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatA.mainTexture = cameraA.targetTexture;

        if (cameraB.targetTexture != null)
            cameraB.targetTexture.Release();

        cameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatB.mainTexture = cameraB.targetTexture;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Disable the render fx if either player camera or portal camera can't see it
        if (VisibleFromCamera(playerCam, portalARenderer) || VisibleFromCamera(cameraA, portalARenderer))
        {
            portalARenderFX.SetActive(true);
        }
        else if (!VisibleFromCamera(playerCam, portalARenderer) && !VisibleFromCamera(cameraA, portalARenderer))
        {
            portalARenderFX.SetActive(false);
        }
        if (VisibleFromCamera(playerCam, portalBRenderer) || VisibleFromCamera(cameraB, portalBRenderer))
        {
            portalBRenderFX.SetActive(true);
        }
        else if(!VisibleFromCamera(playerCam, portalBRenderer) && !VisibleFromCamera(cameraB, portalBRenderer))
        {
            portalBRenderFX.SetActive(false);
        }

    }

    private bool VisibleFromCamera(Camera cam, Renderer renderer)
    {
        // Check if player can see it
        Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(frustrumPlanes, renderer.bounds);
    }

}
