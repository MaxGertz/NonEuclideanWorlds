using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class RenderPlane : MonoBehaviour
{
    private const string OnWorldExitMsg = "OnWorldExit";
    private int _counter;

    private CharacterController _mCharacterController;
    private MeshRenderer _meshRenderer;

    private Player _player;

    private bool _playerIsOverlapping;
    private StartCollectibleSwitcher _startCollectibleSwitcher;
    private float _timeLastTeleport;
    public Material cameraMaterial;

    // Position the player gets teleported to
    public World[] worlds = new World[1];

    private void Start()
    {
        _meshRenderer = gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer;

        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            // Making sure that a new MeshRenderer is created.
            if (_meshRenderer != null) _meshRenderer.material = cameraMaterial;
        }

        _meshRenderer.enabled = true;

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
                // Cosine between Y axis of transform and player position
                var dotProduct = Vector3.Dot(transform.up, portalToPlayer);

                print("Dot:  " + dotProduct);
                // If cosine greater  than 0: The player has moved across/hit the portal
                if (dotProduct < 0.02f)
                    // Teleport him!
                    TeleportPlayer();
            }
            else
            {
                // Player has visited all worlds
                GetComponent<MeshCollider>().isTrigger = false;
                if (_player.collectibleCounter == 18) StartCoroutine(ExtinguishFire());
            }
        }
        else if (_player.collectibleCounter == 18)
        {
            StartCoroutine(ExtinguishFire());
        }
    }

    private void TeleportPlayer()
    {
        var teleportWorld = worlds[_counter];
        if (!teleportWorld.WasVisited)
        {
            var teleporter = teleportWorld.teleporter;

            _player.Teleport(transform, teleporter);
            _player.ActiveWorld = teleportWorld;
            _playerIsOverlapping = false;
            _timeLastTeleport = Time.time;
            _player.SendMessageTo(_startCollectibleSwitcher.gameObject, OnWorldExitMsg);

            Debug.Log("Teleporter - Active world set to: " + _player.ActiveWorld);
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
        if (other.CompareTag("Player"))
            // Deactivating teleporter for 2 seconds after player was just teleported    
            if (Time.time - _timeLastTeleport > 2f)
                _playerIsOverlapping = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _playerIsOverlapping = false;
    }

    private IEnumerator ExtinguishFire()
    {
        DisableCollectibles();
        ActivateSprinkler();
        // Wait for 6 seconds
        yield return new WaitForSeconds(6);
        DeactivateFlames();
    }

    private void DisableCollectibles()
    {
        var collectibles = FindObjectsOfType<Collectible>();
        var managers = FindObjectsOfType<CollectibleManager>();

        foreach (var collectible in collectibles)
        {
            collectible.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void ActivateSprinkler()
    {
        var sprinkler = FindObjectOfType<Sprinkler>();
        if (sprinkler) sprinkler.CreateAndStartSprinkler();
    }

    private void DeactivateFlames()
    {
        var flames = FindObjectsOfType<Fire>();

        // Loop through all flames and deactivate them
        foreach (var flame in flames)
            if (flame.isAlive())
                flame.DeactivateFire();
    }
}