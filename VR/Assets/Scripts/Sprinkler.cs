using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    private GameObject _sprinkler;

    // Prefab from the asset store:
    // https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325
    public GameObject sprinklerPreFab;

    public void CreateAndStartSprinkler()
    {
        // If a sprinkler already exists return
        // There should only be one sprinkler in the scene
        if (_sprinkler) return;

        // Instantiating the sprinkler prefab
        _sprinkler = Instantiate(sprinklerPreFab, transform.position, Quaternion.identity);
    }
}