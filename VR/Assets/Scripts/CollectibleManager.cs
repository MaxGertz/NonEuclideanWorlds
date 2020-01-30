using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private Player _player;
    public Collectible endCollectible;
    public Collectible startCollectible;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (!endCollectible.GetComponent<MeshRenderer>().enabled)
            startCollectible.GetComponent<MeshRenderer>().enabled = true;
    }
}