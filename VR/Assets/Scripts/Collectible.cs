using UnityEngine;
using UnityEngine.Assertions;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            Assert.IsNotNull(meshRenderer, "meshRenderer != null");
            if (meshRenderer.enabled)
            {
                meshRenderer.enabled = false;
                var player = FindObjectOfType<Player>();
                player.CollectibleCounter++;
                Debug.Log("Collectible - Collected! Counter: " + player.CollectibleCounter);
            }
        }
    }
}