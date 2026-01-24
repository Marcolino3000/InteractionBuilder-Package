using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Runtime.Scripts.Interactables
{
    public class InteractableDisplay : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] private Interactable interactable;
        [SerializeField] private TriggerArea triggerArea;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float opacity = 0.7f;
        [SerializeField] private float cooldownBetweenHovers = 1f;
        // [SerializeField] private Material outlineMaterial;
        [SerializeField] private SpriteRenderer spriteRenderer;
        public bool cooldownActive;

        private float currentFadeTime;
        private StyleBackground backgroundImage;
        private VisualElement root;
        [SerializeField] private float currentOpacity;

        private void OnEnable()
        {
            cooldownActive = false;
            currentFadeTime = 0f;
            
            root = GetComponent<UIDocument>().rootVisualElement;
            
            if (interactable.Data.Sprite != null)
            {
                backgroundImage = new StyleBackground(interactable.Data.Sprite);
                root.Query<VisualElement>("icon").First().style.backgroundImage = backgroundImage;
            }
            
            if (root != null)
                HideIcon();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        // private void Awake()
        // {
        //     root = GetComponent<UIDocument>().rootVisualElement;
        //
        //     if (interactable.Data.Sprite != null)
        //     {
        //         backgroundImage = new StyleBackground(interactable.Data.Sprite);
        //         root.Query<VisualElement>("icon").First().style.backgroundImage = backgroundImage;
        //     }
        //
        //     HideIcon();
        // }

        private void ShowIcon()
        {
            var icon = root.Query<VisualElement>("icon").First();
            if (icon != null)
                icon.style.opacity = opacity;
        }

        private void HideIcon()
        {
            var icon = root.Query<VisualElement>("icon").First();
            if (icon != null)
                icon.style.opacity = 0f;
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
            // Color color = outlineMaterial.GetColor("_SolidOutline");
            Material outlineMaterial = spriteRenderer.material;
            Color color = outlineMaterial.GetColor("_SolidOutline");
            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                
                if(fadeIn)
                {
                    currentOpacity = Mathf.Lerp(0f, opacity, currentFadeTime / fadeDuration);
                    color.a = currentOpacity;
                    outlineMaterial.SetColor("_SolidOutline", color);

                }
                else
                {
                    currentOpacity = Mathf.Lerp(opacity, 0f, currentFadeTime / fadeDuration);
                    color.a = currentOpacity;
                    outlineMaterial.SetColor("_SolidOutline", color);

                }
                
                yield return null;
            }

            currentFadeTime = 0f;
        }
        
        private IEnumerator FadeIcon(bool fadeIn)
        {
            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                
                if(fadeIn)
                {
                    currentOpacity = Mathf.Lerp(0f, opacity, currentFadeTime / fadeDuration);
                    root.Query<VisualElement>("icon").First().style.opacity = currentOpacity;

                }
                else
                {
                    currentOpacity = Mathf.Lerp(opacity, 0f, currentFadeTime / fadeDuration);
                    root.Query<VisualElement>("icon").First().style.opacity = currentOpacity;

                }
                
                yield return null;
            }

            currentFadeTime = 0f;
        }
        
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

        public void MarkAsFound()
        {
            root.Query<VisualElement>("icon").First().style.backgroundColor = Color.green; 
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("pointer");
        }    
    }
}