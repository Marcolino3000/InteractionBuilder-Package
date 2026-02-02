using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SauerteigStatusDisplay : MonoBehaviour
{
    [SerializeField] private GameObject Sauerteig;
    [SerializeField] private Image sauerteigImage;
    [SerializeField] private float baseSize;
    [SerializeField] private float scaleFactor;
    [SerializeField] private float animationDuration;

    private void Start()
    {
        // sauerteigImage.enabled = false;
    }

    public void ShowSauerteig()
    {
        Sauerteig.SetActive(true);
    }

    public void UpdateStatus(int status)
    {
        float targetScale = baseSize + status / 10f * scaleFactor;
        StartCoroutine(AnimateScale(targetScale, animationDuration));
    }

    private IEnumerator AnimateScale(float targetY, float duration)
    {
        var size = sauerteigImage.transform.localScale;
        float startY = size.y;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            size.y = Mathf.Lerp(startY, targetY, t);
            sauerteigImage.transform.localScale = size;
            yield return null;
        }
        
        size.y = targetY;
        sauerteigImage.transform.localScale = size;
    }
}
