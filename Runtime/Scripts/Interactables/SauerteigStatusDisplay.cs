using System;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SauerteigStatusDisplay : MonoBehaviour
{
    [SerializeField] private List<Sprite> statusSprites;
    [SerializeField] private Image statusImage;
    [SerializeField] private Image sauerteigImage;
    [SerializeField] private float baseSize;
    [SerializeField] private float scaleFactor;

    private void Start()
    {
        statusImage.enabled = false;
    }

    public void SetStatusSprite(AwarenessLevel awareness)
    {
        statusImage.enabled = true;
        statusImage.sprite = statusSprites[(int)awareness];
    }

    public void UpdateStatus(int status)
    {
        
        var size = sauerteigImage.transform.localScale;
        size.y = baseSize + status / 10f * scaleFactor;
        sauerteigImage.transform.localScale = size;
        
    }
}
