using UnityEngine;

public class World : MonoBehaviour
{
    public bool WasVisited { get; set; }

    // used by VinteR-Receiver to add an offset to y-axis
    public float YOffset => transform.position.y * 1000;
}