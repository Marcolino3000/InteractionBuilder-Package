using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Runtime.Scripts.Interactables
{
    public class GigaGlowManager : MonoBehaviour
    {
        [SerializeField] private bool activateGlow = true;
        [SerializeField] private float Duration;
        [SerializeField] private float maxOpacity;
        [SerializeField] private bool cooldownActive;
        [SerializeField] private float cooldownBetweenGlows;
        
        [SerializeField] private Image glowImage;

        public void Glow()
        {
            if(cooldownActive || !activateGlow)
                return;
            
            StartCoroutine(StartCooldown(Duration));
            StartCoroutine(LerpOpacityInAndOut(Duration));
        }

        public void GlowOneShot(float duration)
        {
            StartCoroutine(StartCooldown(duration));
            StartCoroutine(LerpOpacityInAndOut(duration));
        }
        
        private IEnumerator StartCooldown(float duration)
        {
            cooldownActive = true;
            yield return new WaitForSeconds(duration * 2 + cooldownBetweenGlows);
            cooldownActive = false;
        }
        
        private IEnumerator LerpOpacityInAndOut(float duration)
        {
            yield return StartCoroutine(LerpOpacity(true, duration));
            yield return StartCoroutine(LerpOpacity(false, duration));
        }

        private IEnumerator LerpOpacity(bool lerpIn, float duration)
        {
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                
                var color = glowImage.color;
                
                if (lerpIn)
                    color.a = Mathf.Lerp(0, maxOpacity, elapsed / duration);
                else
                    color.a = Mathf.Lerp(maxOpacity, 0f, elapsed / duration);
                
                glowImage.color = color;
                
                yield return null;
            }
        }
    }
}