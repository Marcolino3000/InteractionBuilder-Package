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

    private void Start()
    {
        statusImage.enabled = false;
    }

    public void SetStatusSprite(AwarenessLevel awareness)
    {
        statusImage.enabled = true;
        statusImage.sprite = statusSprites[(int)awareness];
    }
}
