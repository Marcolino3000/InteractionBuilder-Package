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

        private AsyncOperation _pendingScene;
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
        
        /// <summary>
        /// Loads the scene in the background while the current scene stays fully visible,
        /// then automatically fades out and swaps in the new scene once it is ready.
        /// </summary>
        public static void PreloadAndChangeScene(string sceneName = "Scene 1")
        {
            Instance.StartCoroutine(Instance.PreloadAndChangeSceneRoutine(sceneName));
        }

        private IEnumerator PreloadAndChangeSceneRoutine(string sceneName)
        {
            _pendingScene = SceneManager.LoadSceneAsync(sceneName);

            if (_pendingScene == null)
            {
                Debug.LogError("PreloadAndChangeScene: Scene could not be loaded.");
                yield break;
            }

            // Hold the swap so loading happens in the background with no fade.
            _pendingScene.allowSceneActivation = false;

            // Progress caps at 0.9 while activation is blocked.
            while(_pendingScene.progress < 0.9f)
                yield return null;

            // Let the long load-completion frame pass so the fade doesn't start with
            // an inflated Time.deltaTime and snap straight to black.
            yield return null;

            // Background load is done: fade out, then swap in the new scene.
            SceneFader.Instance.StartFadeOut();
            while(SceneFader.Instance.IsFadingOut)
                yield return null;

            if(deactivateCharacterInScene1)
                marlene.SetActive(true);

            _pendingScene.allowSceneActivation = true;
            _pendingScene = null;
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