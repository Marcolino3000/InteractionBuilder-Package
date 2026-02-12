using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Runtime.Scripts.Interactables
{
    public class InteractableIntroductionDisplay : MonoBehaviour
    {
        [SerializeField] private Image interactableToDisplay;
        [SerializeField] private GigaGlowManager glowManager;
        [SerializeField] private float duration;

        private void Start()
        {
            FindInteractablesInScene();
        }

        private void FindInteractablesInScene()
        {
            var interactables = FindObjectsByType<Interactable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    
            foreach (var interactable in interactables)
            {
                if (interactable.Data != null)
                {
                    interactable.OnInteractionSuccessful += () => OnInteractableDiscovered(interactable.Data);
                }
            }
        }

        private void OnInteractableDiscovered(InteractableState state)
        {
            if (state.Sprite != null)
            {
                var sprite = Sprite.Create(
                    state.Sprite,
                    new Rect(0, 0, state.Sprite.width, state.Sprite.height),
                    new Vector2(0.5f, 0.5f)
                );
                DisplayInteractable(sprite);
            }
        }

        private void DisplayInteractable(Sprite image)
        {
            glowManager.GlowOneShot(duration);

            StartCoroutine(ShowImage(image));
        }

        private IEnumerator ShowImage(Sprite image)
        {
            interactableToDisplay.sprite = image;
            interactableToDisplay.enabled = true;

            yield return new WaitForSeconds(duration);
            
            interactableToDisplay.enabled = false;
        }
    }
}