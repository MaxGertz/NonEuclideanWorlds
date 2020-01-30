using UnityEngine;

public class StartCollectibleSwitcher : MonoBehaviour
{
    public Collectible startCollectible;

    // Start is called before the first frame update
    private void Start()
    {
        startCollectible.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnWorldExit(GameObject sender)
    {
        startCollectible.GetComponent<MeshRenderer>().enabled = true;
    }
}