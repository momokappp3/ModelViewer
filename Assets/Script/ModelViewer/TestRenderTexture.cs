using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRenderTexture : MonoBehaviour
{
    [SerializeField] RawImage _rawImage = null;
    [SerializeField] RenderTextureSystem _renderTexSystem = null;

    private void Start()
    {
        _renderTexSystem.Initialize(1024,1024);
        _renderTexSystem.SetLayerAll();
    }

    private void Update()
    {
        _rawImage.texture = _renderTexSystem.Get();
    }
}
