using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Texture MapTexture;
    public float Speed = 0.1f;
    
    public static bool Paused { get; set; }

    private Material mapMaterial;

    private void fixMapRatio(RawImage rawImage)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float textureAspectRatio = MapTexture.width / (float) MapTexture.height;
        float mapAspectRatio = rectTransform.rect.width / rectTransform.rect.height;
        rawImage.uvRect = new Rect(Vector2.zero, new Vector2(1f, textureAspectRatio / mapAspectRatio));
    }

    private void getMaterial()
    {
        RawImage rawImage = GetComponent<RawImage>();
        mapMaterial = rawImage.material;
        ResetPosition();
    }

    private void Start()
    {
        SetMapTexture(MapTexture);
        getMaterial();
    }

    private void Update()
    {
        if (!Paused)
            mapMaterial.mainTextureOffset += new Vector2(0f, Speed * Time.deltaTime);
    }

    private void OnApplicationQuit()
    {
        ResetPosition();
    }

    public void SetMapTexture(Texture mapTexture)
    {
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = MapTexture;
        fixMapRatio(rawImage);
    }

    public void ResetPosition()
    {
        mapMaterial.mainTextureOffset = new Vector2(0f, 0f);
    }
}
