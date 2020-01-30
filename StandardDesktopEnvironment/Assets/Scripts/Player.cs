using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    // Messages send to object that was hit by raycast
    private const string OnRaycastExitMsg = "OnRaycastExit";
    private const string OnRaycastEnterMsg = "OnRaycastEnter";

    // Camera
    private Camera _camera;
    private GameObject _lastHitGameObject;

    // Saves the timestamp when player was last teleported
    public bool IsTeleported { get; set; }

    public World ActiveWorld { get; set; }

    public int collectibleCounter { get; set; }


    private void Start()
    {
        IsTeleported = false;
        _camera = GetComponentInChildren<Camera>();
        collectibleCounter = 0;
    }

    private void Update()
    {
        // checking which object was hit and sending message to object
        RaycastAndSendMsg();
    }

    public void Teleport(Transform portal, Transform receiver)
    {
        var portalToPlayer = transform.position - portal.position;
        var rotationDiff = -Quaternion.Angle(portal.rotation, receiver.rotation);
        Debug.Log("Rot diff: " + rotationDiff);
        // rotating the player by 180
        // rotationDiff += 180;
        transform.Rotate(Vector3.up, rotationDiff);

        var positionOffset = portalToPlayer;

        // BUG: characterController prevents the teleporation of the player
        //      as a workaround the charactercontroller is disabled
        //      the player is the moved to the new position
        //      the characterController is enabled again
        var m_CharacterController = GetComponent<CharacterController>();
        Assert.IsNotNull(m_CharacterController);
        m_CharacterController.enabled = false;
        transform.position = receiver.position + positionOffset;
        m_CharacterController.enabled = true;

        // Disabling RenderPlane meshrenderer
        var renderActivator = FindObjectOfType<RenderActivator>();
        renderActivator.OnPlayerTeleport();
    
        IsTeleported = !IsTeleported;
    }

    private void RaycastAndSendMsg()
    {
        RaycastHit hit;
        // Creates raycast in the forward direction from player position
        if (Physics.Raycast(transform.position, _camera.transform.forward, out hit, 1.5f))
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
}