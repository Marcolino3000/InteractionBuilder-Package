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
        [SerializeField] private float completeDuration;
        [SerializeField] private float fadeDuration;

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
            glowManager.GlowOneShot(completeDuration * 0.5f);

            StartCoroutine(ShowImage(image));
        }

        private IEnumerator ShowImage(Sprite image)
        {
            interactableToDisplay.sprite = image;
            interactableToDisplay.enabled = true;
            
            float holdDuration = Mathf.Max(0, completeDuration - 2 * fadeDuration);

            yield return StartCoroutine(Fade(true));
            yield return new WaitForSeconds(holdDuration);
            yield return StartCoroutine(Fade(false));
            
            interactableToDisplay.enabled = false;
        }

        private IEnumerator Fade(bool fadeIn)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                var alpha = fadeIn ? Mathf.Lerp(0, 1, t / fadeDuration) : Mathf.Lerp(1, 0, t / fadeDuration);
                interactableToDisplay.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
        }
    }
}