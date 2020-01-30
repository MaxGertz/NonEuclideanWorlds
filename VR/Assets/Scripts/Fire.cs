using UnityEngine;

public class Fire : MonoBehaviour
{
    // Prefab from the asset store:
    // https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325
    private ParticleSystem _particleSystem;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    // Returns the status of the particle system
    // True if enabled
    // Returns false if no particle system is initialized
    public bool IsAlive()
    {
        if (_particleSystem) return _particleSystem.IsAlive();

        return false;
    }

    // Destroys the game object(particle system)
    public void DeactivateFire()
    {
        Destroy(_particleSystem);
        _particleSystem = null;
    }
}