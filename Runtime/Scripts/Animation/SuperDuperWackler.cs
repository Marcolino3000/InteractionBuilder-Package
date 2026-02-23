using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class SuperDuperWackler : MonoBehaviour
    {
        [SerializeField] private float wiggleAmount = 10f;
        [SerializeField] private float wiggleDuration = 0.3f;
        [SerializeField] private int wiggleCount = 3;
        [SerializeField] private float emotionalWiggleAmount = 10f;
        [SerializeField] private float emotionalWiggleDuration = 0.3f;
        [SerializeField] private int emotionalWiggleCount = 3;
        [SerializeField] private List<Interactable> interactablesThatDontTriggerWiggle;

        public void EmotionalWiggle()
        {
            StartCoroutine(EmotionalWiggleCoroutine());
        }
        
        public void Wiggle(Interactable interactable = null)
        {
            if(!InteractableTriggersWiggle(interactable))
                return;
            
            StartCoroutine(WiggleCoroutine());
        }

        private bool InteractableTriggersWiggle(Interactable interactable)
        {
            if(interactable == null)
                return true;
            
            return !interactablesThatDontTriggerWiggle.Contains(interactable);
        }

        private System.Collections.IEnumerator WiggleCoroutine()
        {
            Quaternion startRotation = transform.localRotation;
            float elapsed = 0f;

            while (elapsed < wiggleDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / wiggleDuration;
                
                // Create oscillating rotation with decreasing amplitude
                float angle = Mathf.Sin(progress * Mathf.PI * wiggleCount * 2) * wiggleAmount * (1f - progress);
                transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, angle);
                
                yield return null;
            }

            // Reset to original rotation
            transform.localRotation = startRotation;
        }
        
        private System.Collections.IEnumerator EmotionalWiggleCoroutine()
        {
            Quaternion startRotation = transform.localRotation;
            float elapsed = 0f;

            while (elapsed < emotionalWiggleDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / emotionalWiggleDuration;
                
                // Create oscillating rotation with decreasing amplitude
                float angle = Mathf.Sin(progress * Mathf.PI * emotionalWiggleCount * 2) * emotionalWiggleAmount * (1f - progress);
                transform.localRotation = startRotation * Quaternion.Euler(0f, 0f, angle);
                
                yield return null;
            }

            // Reset to original rotation
            transform.localRotation = startRotation;
        }
    }
}