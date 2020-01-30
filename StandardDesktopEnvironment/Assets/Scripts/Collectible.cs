using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer.enabled)
            {
                Debug.Log("Hit collectible");
                meshRenderer.enabled = false;
                var player = FindObjectOfType<Player>();
                player.collectibleCounter++;
                Debug.Log("Collectible - Collected! Counter: " + player.collectibleCounter);
            }
        }
    }
}