using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Scripts.Interactables
{
    public class InteractableDisplay : MonoBehaviour
    {
        [SerializeField] private InteractableState state;
        [SerializeField] private TriggerArea triggerArea;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float opacity = 0.7f;

        private float currentFadeTime;
        private StyleBackground backgroundImage;
        private VisualElement root;

        private void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            
            backgroundImage = new StyleBackground(state.Sprite);
            root.Query<VisualElement>("icon").First().style.backgroundImage = backgroundImage;

            triggerArea.OnTriggerEntered += (PlayerController) => 
            {
                Show();
            };
            
            triggerArea.OnTriggerExited += Hide;

            StartCoroutine(FadeIcon(false));
        }

        private void Hide()
        {
            StartCoroutine(FadeIcon(false));
        }

        private void Show()
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
    }
}