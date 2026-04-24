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
        [SerializeField] private float standardOutlineFadeDuration = 0.05f;
        [SerializeField] private float minOpacity = 0;
        [SerializeField] private float maxOpacity = 0.7f;
        [SerializeField] private float specialOutlineFadeDuration = 2;
        [SerializeField] private float cooldownBetweenHovers = 1;
        [SerializeField] private Color specialInteractablesColor;
        [SerializeField] private Color standardInteractablesColor;

        [Header("References")]
        [SerializeField] private Interactable interactable;
        [SerializeField] private TriggerArea triggerArea;
        [SerializeField] private string shaderColorPropertyRef;
        [SerializeField] private string outlineAlphaRef;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private float currentFadeTime;
        private Color currentColor;
        private bool showSpecialOutline;

        public void ShowStandardOutline()
        {
            StopAllCoroutines();
            currentColor = standardInteractablesColor;
            Show(standardOutlineFadeDuration);
        }

        public void HideStandardOutline()
        {
            StopAllCoroutines();
            Hide(standardOutlineFadeDuration);
        }

        public void ShowStaticSpecialOutline()
        {
            StopAllCoroutines();
            currentColor = specialInteractablesColor;
            Show(specialOutlineFadeDuration);
        }
        
        public void HideStaticSpecialOutline()
        {
            StopAllCoroutines();
            Hide(specialOutlineFadeDuration);
        }

        public void ShowPulsatingSpecialOutline()
        {
            currentColor = specialInteractablesColor;
            showSpecialOutline = true;
            StartCoroutine(TriggerSpecialOutline());
        }

        private void HideSpecialOutline()
        {
            showSpecialOutline = false;
            StopAllCoroutines();
        }

        private IEnumerator TriggerSpecialOutline()
        {
            while (showSpecialOutline)
            {
                StartFadingOutlineInAndOut();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void StartFadingOutlineInAndOut()
        {
            if (cooldownActive) 
                return;
            
            // StartCoroutine(StartCooldownCoroutine());
            StartCoroutine(QuickFadeInAndOut());
        }

        private IEnumerator StartCooldownCoroutine()
        {
            cooldownActive = true;
            yield return new WaitForSeconds(standardOutlineFadeDuration * 2 + cooldownBetweenHovers);
            cooldownActive = false;
        }

        private IEnumerator QuickFadeInAndOut()
        {
            cooldownActive = true;
            yield return FadeOutline(true, specialOutlineFadeDuration);
            yield return FadeOutline(false, specialOutlineFadeDuration);
            cooldownActive = false;
        }

        private IEnumerator FadeOutline(bool fadeIn, float fadeDuration)
        {
            if (spriteRenderer == null)
                yield break;
            
            Material outlineMaterial = spriteRenderer.material;
            outlineMaterial.SetColor(shaderColorPropertyRef, currentColor);

            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;

                currentOpacity = fadeIn ?
                    Mathf.Lerp(minOpacity, maxOpacity, currentFadeTime / fadeDuration) :
                    Mathf.Lerp(maxOpacity, minOpacity, currentFadeTime / fadeDuration);

                Color newcolor = new Color(currentColor.r, currentColor.g, currentColor.b, currentOpacity);
                outlineMaterial.SetColor(shaderColorPropertyRef, newcolor);
                
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

        private void Hide(float duration)
        {
            StartCoroutine(FadeOutline(false, duration));
        }

        private void Show(float duration)
        {
            StartCoroutine(FadeOutline(true, duration));
        }
    }
}