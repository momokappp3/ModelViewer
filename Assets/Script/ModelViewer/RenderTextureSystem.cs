using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderTextureSystem : MonoBehaviour
{
    [SerializeField] private Camera _camera = null;

    private RenderTexture _renderTexture = null;
    public int _layerNum = -1;

    private void Start()
    {
        if(_layerNum == -1)
        {
            GetLayerNumber();
        }

        if(_camera == null)
        {
            TryGetComponent(out _camera);
        }
        _camera.cullingMask = 1 << _layerNum;

    }

    private void GetLayerNumber()
    {
        _layerNum = LayerMask.NameToLayer("RenderTexture");
    }

    private void OnDestroy()
    {
        Release();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if(_renderTexture == null)
        {
            return;
        }

        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = _renderTexture;

        Graphics.Blit(source, _renderTexture);
        Graphics.Blit(_renderTexture, dest);

        RenderTexture.active = currentActiveRT;
    }

    public RenderTexture Get()
    {
        return _renderTexture;
    }

    public bool Initialize(int width, int height)
    {
        Release();

        _renderTexture = new RenderTexture(width, height, 32);

        if (_renderTexture == null)
        {
            return false;
        }

        _renderTexture.Create();

        GetLayerNumber();

        return true;
    }

    public bool Release()
    {
        if (_renderTexture == null)
        {
            return false;
        }

        _renderTexture.Release();

        return true;
    }

    public void SetLayerAll()
    {
        Transform trans = this.transform;

        SetLayer(ref trans);
    }

    private void SetLayer(ref Transform trans)
    {
        for (var i = 0; i < trans.childCount; ++i)
        {
            Transform child = trans.GetChild(i);

            child.gameObject.layer = _layerNum;

            SetLayer(ref child);
        }
    }
}