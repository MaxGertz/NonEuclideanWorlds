using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR;

public class RenderPlane : MonoBehaviour
{
    private const string OnWorldExitMsg = "OnWorldExit";
    private int _counter;
    private CharacterController _mCharacterController;
    private MeshRenderer _meshRenderer;

    private Player _player;

    // Switched to true if player is hitting trigger
    private bool _playerIsOverlapping;
    private float _lastTeleport;
    private StartCollectibleSwitcher _startCollectibleSwitcher;

    public Material cameraMaterial;

    // Array of worlds player gets teleported to when hitting the collider
    public World[] worlds = new World[1];

    private void Start()
    {
        _meshRenderer = gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        _lastTeleport = 0;

        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            // Making sure that a new MeshRenderer is created.
            if (_meshRenderer != null) _meshRenderer.material = cameraMaterial;
        }

        _meshRenderer.enabled = false;

        _player = FindObjectOfType<Player>();
        Assert.IsNotNull(_player, "Player not found in scene.");
        _startCollectibleSwitcher = FindObjectOfType<StartCollectibleSwitcher>();
    }

    private void Update()
    {
        if (_playerIsOverlapping)
        {
            if (_counter != -1)
            {
                var portalToPlayer = _player.transform.position - transform.position;
                // cosine between Y axis of transform and player position
                var dotProduct = Vector3.Dot(transform.up, portalToPlayer);
                Debug.Log("DOT: " + dotProduct);
                // If cosine greater  than 0: The player has moved across/hit the portal
                bool t = false;
                if (dotProduct <= 0.02f)
                    // Teleport him!
                    TeleportPlayer();
            }
            else
            {
                // Player has visited all worlds
                if (_player.CollectibleCounter >= 17 && !_player.IsTeleported)
                {
                    StartCoroutine(ExtinguishFire());
                }
            }
        }
        else if (_counter == -1 && !_player.IsTeleported && _player.CollectibleCounter >= 16)
        {
            StartCoroutine(ExtinguishFire());
        }
    }

    private void TeleportPlayer()
    {
        if (!worlds[_counter].WasVisited)
        {
            _player.ActiveWorld = worlds[_counter];
            _player.IsTeleported = true;

            Debug.Log("Teleporter - Player teleported to " + _player.ActiveWorld.name);
            _playerIsOverlapping = false;
            _player.LastTeleported = Time.time;
            _player.SendMessageTo(_startCollectibleSwitcher.gameObject, OnWorldExitMsg);
        }

        if (_counter < worlds.Length - 1)
        {
            worlds[_counter].WasVisited = true;
            _counter++;
            Debug.Log("Teleporter - Next world set to: " + worlds[_counter].name);
        }
        else
        {
            _counter = -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _counter != -1)
            // Deactivating teleporter for 4 seconds after player was just teleported    
            if (Time.time - _lastTeleport > 4f)
                _playerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _playerIsOverlapping = false;
    }

    private IEnumerator ExtinguishFire()
    {
        ActivateSprinkler();
        // Wait for 10 seconds
        yield return new WaitForSeconds(10);
        DeactivateFlames();
        yield return new WaitForSeconds(2);
        SteamVR_Fade.Start(Color.black, 5, false);
    }

    private void ActivateSprinkler()
    {
        var sprinkler = FindObjectOfType<Sprinkler>();
        if (sprinkler) sprinkler.CreateAndStartSprinkler();
    }

    private void DeactivateFlames()
    {
        var fires = FindObjectsOfType<Fire>();

        // Loop through all flames and deactivate them
        foreach (var fire in fires)
            if (fire.IsAlive())
                fire.DeactivateFire();
    }
}