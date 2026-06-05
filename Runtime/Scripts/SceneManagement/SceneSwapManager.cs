using System.Collections;
using Runtime.Scripts.Interactables;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneSwapManager : MonoBehaviour
    {
        public static SceneSwapManager Instance { get; private set; }

        [SerializeField] private float fadeDuration;
        [SerializeField] private SceneSetup sceneSetup;
        
        [SerializeField] private GameObject marlene;
        [SerializeField] private bool deactivateCharacterInScene1;
        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            
            if(deactivateCharacterInScene1)
                marlene.SetActive(false);
        }
        
        public static void ChangeScene(string sceneName = "Scene 1")
        {
            Instance.StartCoroutine(Instance.FadeOutAndChangeScene(sceneName));
        }
        
        public static void LoadFirstScene()
        {
            Instance.StartCoroutine(Instance.FadeOutAndChangeScene("Scene 1"));
        }
        
        private IEnumerator FadeOutAndChangeScene(string sceneName)
        {
            SceneFader.Instance.StartFadeOut();
            while(SceneFader.Instance.IsFadingOut)
                yield return null;

            if(deactivateCharacterInScene1)
                marlene.SetActive(true);
            
            SceneManager.LoadScene(sceneName);
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneFader.Instance.StartFadeIn();
            sceneSetup.SetupScene();
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