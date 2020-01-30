using System;
using UnityEngine;

public class PortalCameraRig : MonoBehaviour

{
    private const string CenterEyeAnchorName = "CenterEyeAnchor";
    private LayerMask _layer;

    private Player _player;
    public Transform destinationPortal;
    public float mClipPlaneOffset = 0.15f;
    public Transform ownPortal;

    public RenderTexture renderTexture;
    private Transform CenterEyeAnchor { get; set; }
    public Camera CenterEyeCamera { get; private set; }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        gameObject.tag = "RenderCamera";
        _layer = 1 << LayerMask.NameToLayer("FakeWorld");


        if (CenterEyeAnchor == null)
            CenterEyeAnchor = ConfigureAnchor(null, CenterEyeAnchorName);

        if (CenterEyeCamera == null)
        {
            CenterEyeCamera = CenterEyeAnchor.GetComponent<Camera>();

            CenterEyeCamera = CenterEyeAnchor.gameObject.AddComponent<Camera>();
            CenterEyeAnchor.name = "portalCamera";
        }

        SetupCameraProperties(CenterEyeCamera);
    }


    private void FixedUpdate()
    {
        UpdatePosition();
        UpdateRotation(CenterEyeAnchor, _player.RightEyeCamera);
        //SetupProjectionMatrix(CenterEyeCamera, _player.RightEyeCamera);
    }


    private void OnPostRender()
    {
        // Discard the colour and depth buffer of the rendertextures
        CenterEyeCamera.targetTexture.DiscardContents();
    }

    private void SetupCameraProperties(Camera camera)
    {
        camera.fieldOfView = 60;
        camera.depth = 0;
        //camera.clearFlags = CameraClearFlags.Depth;
        camera.useOcclusionCulling = false;
        camera.allowMSAA = false;
        camera.allowHDR = false;
        camera.cullingMask = _layer;
        camera.targetTexture = renderTexture;
    }

    private void UpdatePosition()
    {
        var playerOffsetFromPortal = _player.transform.position - destinationPortal.position;

        var newPosition = ownPortal.position + playerOffsetFromPortal;
        newPosition = new Vector3(newPosition.x, newPosition.y, newPosition.z);
        transform.position = newPosition;
    }

    private void SetupProjectionMatrix(Camera camera, Camera playerCamera)
    {
        var clipPlane =
            CameraSpacePlane(camera, ownPortal.position, destinationPortal.forward, 1.0f);
        var projection = playerCamera.projectionMatrix;
        CalculateObliqueMatrix(ref projection, clipPlane);
        camera.projectionMatrix = projection;
    }

    private void UpdateRotation(Transform camera, Camera playerCamera)
    {
        var diffCamGameObject = _player.transform.position - playerCamera.transform.position;

        camera.position = transform.position + -diffCamGameObject;


        var newRotation = playerCamera.transform.rotation;
        newRotation = Quaternion.Euler(
            newRotation.eulerAngles.x,
            newRotation.eulerAngles.y,
            newRotation.eulerAngles.z);

        camera.rotation = Quaternion.Slerp(newRotation, playerCamera.transform.rotation, 1f);
        //camera.rotation = newRotation;
    }


    protected virtual Transform ConfigureAnchor(Transform root, string name)
    {
        var anchor = root != null ? root.Find(name) : null;

        if (anchor == null) anchor = transform.Find(name);

        if (anchor == null) anchor = new GameObject(name).transform;

        anchor.name = name;
        anchor.parent = root != null ? root : transform;
        anchor.localScale = Vector3.one;
        anchor.localPosition = Vector3.zero;
        anchor.localRotation = Quaternion.identity;

        return anchor;
    }

    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        var offsetPos = pos + normal * mClipPlaneOffset;
        var m = cam.worldToCameraMatrix;
        var cpos = m.MultiplyPoint(offsetPos);
        var cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z - 2, -Vector3.Dot(cpos, cnormal));
    }

    // Adjusts the given projection matrix so that near plane is the given clipPlane
    // clipPlane is given in camera space. See article in Game Programming Gems 5 and
    // http://aras-p.info/texts/obliqueortho.html
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        var q = projection.inverse * new Vector4(
                    sgn(clipPlane.x),
                    sgn(clipPlane.y),
                    1.0f,
                    1.0f
                );
        var c = clipPlane * (2.0F / Vector4.Dot(clipPlane, q));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    // Taken from: http://wiki.unity3d.com/index.php/MirrorReflection3
    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }
}