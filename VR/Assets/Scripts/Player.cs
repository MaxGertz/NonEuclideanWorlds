using UnityEngine;

public class Player : MonoBehaviour
{
    // Messages send to object that was hit by raycast
    private const string OnRaycastExitMsg = "OnRaycastExit";
    private const string OnRaycastEnterMsg = "OnRaycastEnter";


    private const string LeftEyeAnchorName = "LeftEyeAnchor";
    private const string RightEyeAnchorName = "RightEyeAnchor";
    private GameObject _lastHitGameObject;

    // Saves the timestamp when player was last teleported
    public float LastTeleported { get; set; }
    public bool IsTeleported { get; set; }
    public int CollectibleCounter { get; set; }

    public World ActiveWorld { get; set; }

    // Cameras & Anchors
    private Transform LeftEyeAnchor { get; set; }
    private Transform RightEyeAnchor { get; set; }
    public Camera LeftEyeCamera { get; private set; }
    public Camera RightEyeCamera { get; private set; }


    private void Start()
    {
        EnsureGameObjectIntegrity();
        // Player was not teleported before -> setting value to 0;
        LastTeleported = 0f;
        IsTeleported = false;
        LeftEyeCamera.nearClipPlane = 0.001f;
        RightEyeCamera.nearClipPlane = 0.001f;
    }

    private void Update()
    {
        // checking which object was hit and sending message to object
        RaycastAndSendMsg();
    }

    private void RaycastAndSendMsg()
    {
        RaycastHit hit;
        // Creates raycast in the forward direction from player position
        if (Physics.Raycast(transform.position, LeftEyeCamera.transform.forward, out hit, 1.5f))
        {
            // Checking which object we hit
            var currentHitGameObject = hit.collider.gameObject;
            // Checking if we hit another object then the last time
            if (_lastHitGameObject != currentHitGameObject)
            {
                // Send message that the object is in the view of the player
                // Objects handle this messages
                SendMessageTo(_lastHitGameObject, OnRaycastExitMsg);
                SendMessageTo(currentHitGameObject, OnRaycastEnterMsg);
                // Setting the new hit object to the last hit object
                _lastHitGameObject = currentHitGameObject;
            }
        }
        else
        {
            // Notify the last hit object that it is no longer in the view of the player
            SendMessageTo(_lastHitGameObject, OnRaycastExitMsg);
            _lastHitGameObject = null;
        }
    }

    public void SendMessageTo(GameObject target, string msg)
    {
        if (target) target.SendMessage(msg, gameObject, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void EnsureGameObjectIntegrity()
    {
        if (LeftEyeAnchor == null) LeftEyeAnchor = ConfigureAnchor(LeftEyeAnchorName);

        if (RightEyeAnchor == null) RightEyeAnchor = ConfigureAnchor(RightEyeAnchorName);

        if (LeftEyeCamera == null || RightEyeCamera == null)
        {
            LeftEyeCamera = LeftEyeAnchor.GetComponent<Camera>();
            RightEyeCamera = RightEyeAnchor.GetComponent<Camera>();

            if (LeftEyeCamera == null)
            {
                LeftEyeCamera = LeftEyeAnchor.gameObject.AddComponent<Camera>();
                LeftEyeCamera.tag = "MainCamera";
                LeftEyeCamera.depth = 1;
            }

            if (RightEyeCamera == null)
            {
                RightEyeCamera = RightEyeAnchor.gameObject.AddComponent<Camera>();
                RightEyeCamera.tag = "MainCamera";
                RightEyeCamera.depth = 1;
            }

            LeftEyeCamera.stereoTargetEye = StereoTargetEyeMask.Left;
            RightEyeCamera.stereoTargetEye = StereoTargetEyeMask.Right;
        }
    }

    protected virtual Transform ConfigureAnchor(string name)
    {
        var anchor = transform.Find(name);

        if (anchor == null) anchor = new GameObject(name).transform;

        anchor.name = name;
        anchor.parent = transform;
        anchor.localScale = Vector3.one;
        anchor.localPosition = Vector3.zero;
        anchor.localRotation = Quaternion.identity;

        return anchor;
    }
}