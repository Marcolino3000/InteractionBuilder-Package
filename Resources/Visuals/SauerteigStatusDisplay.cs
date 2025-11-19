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
        statusImage.sprite = statusSprites[0];
    }

    public void SetStatusSprite(AwarenessLevel awareness)
    {
        statusImage.sprite = statusSprites[(int)awareness];
    }
}
