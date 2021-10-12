using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    [SerializeField] private Camera playerCam;
    [SerializeField] private Camera cameraB;
    [SerializeField] private Material cameraMatB;
    [SerializeField] private GameObject portalBRenderFX;
    private Renderer portalBRenderer;

    private void Awake()
    {
        playerCam = Camera.main;
        portalBRenderer = portalBRenderFX.GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (cameraB.targetTexture != null)
            cameraB.targetTexture.Release();

        cameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMatB.mainTexture = cameraB.targetTexture;
    }

    // Update is called once per frame
    void LateUpdate()
    {
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
