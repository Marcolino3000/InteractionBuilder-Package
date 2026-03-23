using System;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance { get; private set; }
        
        [SerializeField] private Image fadeOutImage;
        [Range(0.1f, 10f), SerializeField] private float fadeSpeed;
        [SerializeField] private Color fadeColor = Color.black;
        
        public bool IsFadingIn { get; private set; }
        public bool IsFadingOut { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            fadeColor.a = 0f;
        }

        private void Update()
        {
            if (IsFadingOut)
            {
                if(fadeOutImage.color.a < 1f)
                {
                    fadeColor.a += Time.deltaTime * fadeSpeed;
                    fadeOutImage.color = fadeColor;
                }
                else
                {
                    IsFadingOut = false;
                }
            }

            if (IsFadingIn)
            {
                if (fadeOutImage.color.a > 0f)
                {
                    fadeColor.a -= Time.deltaTime * fadeSpeed;
                    fadeOutImage.color = fadeColor;
                }
                else
                {
                    IsFadingIn = false;
                }
            }
        }

        public void StartFadeOut()
        {
            fadeOutImage.color = fadeColor;
            IsFadingOut = true;
        }

        public void StartFadeIn()
        {
            fadeOutImage.color = fadeColor;
            IsFadingIn = true;
        }
    }
}