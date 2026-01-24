using System;
using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Runtime.Scripts.Interactables
{
    public class GigaGlowManager : MonoBehaviour
    {
        [SerializeField] private float duration;
        [SerializeField] private float maxOpacity;
        [SerializeField] private bool cooldownActive;
        [SerializeField] private float cooldownBetweenGlows;
        
        [SerializeField] private Image glowImage;

        public void Glow()
        {
            if(cooldownActive)
                return;
            
            StartCoroutine(StartCooldown());
            StartCoroutine(LerpOpacityInAndOut());
        }
        
        private IEnumerator StartCooldown()
        {
            cooldownActive = true;
            yield return new WaitForSeconds(duration * 2 + cooldownBetweenGlows);
            cooldownActive = false;
        }

        private IEnumerator LerpOpacityInAndOut()
        {
            yield return StartCoroutine(LerpOpacity(true));
            yield return StartCoroutine(LerpOpacity(false));
        }

        private IEnumerator LerpOpacity(bool lerpIn)
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