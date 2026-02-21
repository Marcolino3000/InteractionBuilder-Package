using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Runtime.Scripts.Interactables
{
    public class InteractableDisplay : MonoBehaviour
    {
        [SerializeField] private Interactable interactable;
        [SerializeField] private TriggerArea triggerArea;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float opacity = 0.7f;
        [SerializeField] private float cooldownBetweenHovers = 1f;
        // [SerializeField] private Material outlineMaterial;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private string shaderColorPropertyRef;
        [SerializeField] private string outlineAlphaRef;
        public bool cooldownActive;

        private float currentFadeTime;
        // private StyleBackground backgroundImage;
        // private VisualElement root;
        [SerializeField] private float currentOpacity;

        private void OnEnable()
        {
            cooldownActive = false;
            currentFadeTime = 0f;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

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

        public void Hide()
        {
            StartCoroutine(FadeOutline(false));
        }

        public void Show()
        {
            StartCoroutine(FadeOutline(true));
        }
        
        private IEnumerator FadeOutline(bool fadeIn)
        {
            Material outlineMaterial = spriteRenderer.material;
            // var outlineAlpha = outlineMaterial.GetFloat(outlineAlphaRef);
            
            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                
                currentOpacity = fadeIn ? 
                    Mathf.Lerp(0f, opacity, currentFadeTime / fadeDuration) : 
                    Mathf.Lerp(opacity, 0f, currentFadeTime / fadeDuration);

                outlineMaterial.SetFloat(outlineAlphaRef, currentOpacity);

                yield return null;
            }

            currentFadeTime = 0f;
        }

        // private IEnumerator FadeOutline(bool fadeIn)
        // {
        //     // Color color = outlineMaterial.GetColor("_SolidOutline");
        //     Material outlineMaterial = spriteRenderer.material;
        //     Color color = outlineMaterial.GetColor(shaderColorPropertyRef);
        //     while (currentFadeTime < fadeDuration)
        //     {
        //         currentFadeTime += Time.deltaTime;
        //         
        //         if(fadeIn)
        //         {
        //             currentOpacity = Mathf.Lerp(0f, opacity, currentFadeTime / fadeDuration);
        //             color.a = currentOpacity;
        //             outlineMaterial.SetColor(shaderColorPropertyRef, color);
        //
        //         }
        //         else
        //         {
        //             currentOpacity = Mathf.Lerp(opacity, 0f, currentFadeTime / fadeDuration);
        //             color.a = currentOpacity;
        //             outlineMaterial.SetColor(shaderColorPropertyRef, color);
        //
        //         }
        //         
        //         yield return null;
        //     }
        //
        //     currentFadeTime = 0f;
        // }
        
        private Texture2D ChangeTextureOpacity(Texture2D originalTexture, float opacity)
        {
            Texture2D newTexture = new Texture2D(originalTexture.width, originalTexture.height, TextureFormat.RGBA32, false);
            Color[] pixels = originalTexture.GetPixels();
    
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i].a = opacity;
            }
    
            newTexture.SetPixels(pixels);
            newTexture.Apply();
    
            return newTexture;
        }
    }
}