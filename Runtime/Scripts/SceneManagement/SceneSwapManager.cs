using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneSwapManager : MonoBehaviour
    {
        public static SceneSwapManager Instance { get; private set; }

        [SerializeField] private float fadeDuration;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
        }
        
        public void ChangeScene(string sceneName = "Scene 1")
        {
            Instance.StartCoroutine(Instance.FadeOutAndChangeScene(sceneName));
        }
        
        private IEnumerator FadeOutAndChangeScene(string sceneName)
        {
            SceneFader.Instance.StartFadeOut();
            while(SceneFader.Instance.IsFadingOut)
                yield return null;

            SceneManager.LoadScene(sceneName);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneFader.Instance.StartFadeIn();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}