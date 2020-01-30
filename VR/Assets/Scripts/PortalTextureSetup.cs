using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Material cameraMatA;
    public PortalCameraRig portalCameraRig;

    private void Start()
    {
        cameraMatA.mainTexture = portalCameraRig.CenterEyeCamera.targetTexture;
    }
}