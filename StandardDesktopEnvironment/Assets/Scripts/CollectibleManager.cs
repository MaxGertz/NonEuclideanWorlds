using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private Player _player;
    public Collectible startCollectible;
    public Collectible endCollectible;

    // Start is called before the first frame update
    private void Start()
    {
        startCollectible.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!endCollectible.GetComponent<MeshRenderer>().enabled)
            startCollectible.GetComponent<MeshRenderer>().enabled = true;
    }
}