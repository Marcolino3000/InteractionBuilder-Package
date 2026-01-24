using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class SuperDuperWackler : MonoBehaviour
    {
        [SerializeField] private float wiggleAmount = 10f;
        [SerializeField] private float wiggleDuration = 0.3f;
        [SerializeField] private int wiggleCount = 3;

        public void Wiggle()
        {
            StartCoroutine(WiggleCoroutine());
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
    }
}