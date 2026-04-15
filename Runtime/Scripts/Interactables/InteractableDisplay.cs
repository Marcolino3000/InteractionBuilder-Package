using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Runtime.Scripts.Interactables
{
    public class InteractableDisplay : MonoBehaviour
    {
        [Header("Debug")]
        public bool cooldownActive;
        [SerializeField] private float currentOpacity;
        
        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float minOpacity = 0.2f;
        [SerializeField] private float maxOpacity = 0.7f;
        [SerializeField] private float cooldownBetweenHovers = 1f;

        [Header("References")]
        [SerializeField] private Interactable interactable;
        [SerializeField] private TriggerArea triggerArea;
        [SerializeField] private string shaderColorPropertyRef;
        [SerializeField] private string outlineAlphaRef;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private float currentFadeTime;

        public void TriggerHoverEffect()
        {
            if (cooldownActive) 
                return;
            
            StartCoroutine(StartCooldownCoroutine());
            StartCoroutine(QuickFadeInAndOut());
        }

        private IEnumerator StartCooldownCoroutine()
        {
            cooldownActive = true;
            yield return new WaitForSeconds(fadeDuration * 2 + cooldownBetweenHovers);
            cooldownActive = false;
        }

        private IEnumerator QuickFadeInAndOut()
        {
            yield return FadeOutline(true);
            yield return FadeOutline(false);
        }

        private IEnumerator FadeOutline(bool fadeIn)
        {
            Material outlineMaterial = spriteRenderer.material;
            
            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                
                currentOpacity = fadeIn ? 
                    Mathf.Lerp(minOpacity, maxOpacity, currentFadeTime / fadeDuration) : 
                    Mathf.Lerp(maxOpacity, minOpacity, currentFadeTime / fadeDuration);

                outlineMaterial.SetFloat(outlineAlphaRef, currentOpacity);

                yield return null;
            }

            currentFadeTime = 0f;
        }

        private void OnEnable()
        {
            cooldownActive = false;
            currentFadeTime = 0f;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void Hide()
        {
            StartCoroutine(FadeOutline(false));
        }

        public void Show()
        {
            StartCoroutine(FadeOutline(true));
        }
    }
}