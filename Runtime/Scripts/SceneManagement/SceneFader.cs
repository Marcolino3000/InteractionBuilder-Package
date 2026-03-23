using System;
using UnityEngine;
using UnityEngine.UI;

namespace SceneManagement
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance { get; private set; }
        
        [SerializeField] private Image fadeImage;
        [Range(0.1f, 10f), SerializeField] private float fadeSpeed;
        [SerializeField] private Color fadeColor = Color.black;
        
        public bool IsFadingIn { get; private set; }
        public bool IsFadingOut { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            fadeColor.a = 0f;
            fadeImage.enabled = false;
        }

        private void Update()
        {
            if (IsFadingOut)
            {
                if(fadeImage.color.a < 1f)
                {
                    fadeColor.a += Time.deltaTime * fadeSpeed;
                    fadeImage.color = fadeColor;
                }
                else
                {
                    IsFadingOut = false;
                }
            }

            if (IsFadingIn)
            {
                if (fadeImage.color.a > 0f)
                {
                    fadeColor.a -= Time.deltaTime * fadeSpeed;
                    fadeImage.color = fadeColor;
                }
                else
                {
                    IsFadingIn = false;
                    fadeImage.enabled = false;
                }
            }
        }

        public void StartFadeOut()
        {
            fadeImage.enabled = true;
            fadeImage.color = fadeColor;
            IsFadingOut = true;
        }

        public void StartFadeIn()
        {
            fadeImage.enabled = true;
            fadeImage.color = fadeColor;
            IsFadingIn = true;
        }
    }
}