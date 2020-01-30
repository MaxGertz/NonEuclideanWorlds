using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class RenderActivator : MonoBehaviour
{
    [FormerlySerializedAs("RenderPlane")] public GameObject renderPlane;

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
            if (!_meshRenderer.enabled)
            {
                _meshRenderer.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _meshRenderer.enabled = false;
        }
    }
}