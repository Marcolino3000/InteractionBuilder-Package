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
            float startScale = fadeIn ? 0f : 1f;
            float endScale = fadeIn ? 1f : 0f;
            Vector3 initialScale = Vector3.one * startScale;
            Vector3 targetScale = Vector3.one * endScale;
            Color baseColor = interactableToDisplay.color;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                float progress = t / fadeDuration;
                float alpha = fadeIn ? Mathf.Lerp(0, 1, progress) : Mathf.Lerp(1, 0, progress);
                interactableToDisplay.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                interactableToDisplay.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);
                yield return null;
            }
            float finalAlpha = fadeIn ? 1f : 0f;
            interactableToDisplay.color = new Color(baseColor.r, baseColor.g, baseColor.b, finalAlpha);
            interactableToDisplay.transform.localScale = targetScale;
        }
    }
}