using UnityEngine;
using UnityEngine.Assertions;

public class PortalTeleporter : MonoBehaviour
{
    private float _lastTeleport;
    private CharacterController _mCharacterController;

    private Player _player;

    private bool _playerIsOverlapping;

    // PlayerObject
    private GameObject _playerObject;
    private Transform _playerTransform;

    // Position the player gets teleported to
    public Transform receiver;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        Assert.IsNotNull(_player, "Player not found in scene.");
        //Searching for the Player
        _playerObject = GameObject.FindWithTag("Player");
        _playerTransform = _playerObject.transform;
        _lastTeleport = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        // Player hits colliderPane -> Teleport player to other portal
        if (_playerIsOverlapping)
        {
            var portalToPlayer = _playerTransform.position - transform.position;
            // cosine between Y axis of transform and player position
            var dotProduct = Vector3.Dot(transform.up, portalToPlayer);

            // If cosine greater  than 0: The player has moved across/hit the portal
            if (dotProduct > 0f)
            {
                // Teleport him!
                _player.Teleport(transform, receiver);
                _playerIsOverlapping = false;
                _lastTeleport = Time.time;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit teleporter " + name + " " + other.tag);

        if (other.CompareTag("Player"))
            // Deactivating teleporter for 2 seconds after player was just teleported    
            if (Time.time - _lastTeleport > 2f)
                _playerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _playerIsOverlapping = false;
    }
}