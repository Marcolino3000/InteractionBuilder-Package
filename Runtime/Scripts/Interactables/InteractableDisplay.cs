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
        [SerializeField] private float cooldownBetweenHovers = 3f;

        private float currentFadeTime;
        private StyleBackground backgroundImage;
        private VisualElement root;
        private bool cooldownActive;

        private void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            if (interactable.Data.Sprite != null)
            {
                backgroundImage = new StyleBackground(interactable.Data.Sprite);
                root.Query<VisualElement>("icon").First().style.backgroundImage = backgroundImage;
            }

            // triggerArea.OnPlayerEntered += (PlayerController) => 
            // {
            //     Show();
            // };
            //
            // triggerArea.OnPlayerExited += Hide;

            Hide();
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
            yield return new WaitForSeconds(cooldownBetweenHovers);
            cooldownActive = false;
        }

        public IEnumerator QuickFadeInAndOut()
        {
            yield return FadeIcon(true);
            yield return FadeIcon(false);
        }

        public void Hide()
        {
            StartCoroutine(FadeIcon(false));
        }

        public void Show()
        {
            StartCoroutine(FadeIcon(true));
        }

        private IEnumerator FadeIcon(bool fadeIn)
        {
            while (currentFadeTime < fadeDuration)
            {
                currentFadeTime += Time.deltaTime;
                
                if(fadeIn)
                    root.Query<VisualElement>("icon").First().style.opacity = Mathf.Lerp(0f, opacity, currentFadeTime / fadeDuration);
                else
                    root.Query<VisualElement>("icon").First().style.opacity = Mathf.Lerp(opacity, 0f, currentFadeTime / fadeDuration);
                
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