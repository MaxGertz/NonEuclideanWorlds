using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;

public class RenderActivator : MonoBehaviour
{
    public GameObject renderPlane;
    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _meshRenderer = renderPlane.GetComponent<MeshRenderer>();
        Assert.IsNotNull(_meshRenderer, "_renderPlane != null");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _meshRenderer.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _meshRenderer.enabled = false;
        }
    }

    public void OnPlayerTeleport()
    {
        _meshRenderer.enabled = false;
    }
}