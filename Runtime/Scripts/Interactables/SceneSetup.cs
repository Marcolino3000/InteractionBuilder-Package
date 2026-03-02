using System;
using System.Collections;
using System.Linq;
using Runtime.Scripts.Core;
using Runtime.Scripts.Utility;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class SceneSetup : MonoBehaviour
    {
        [SerializeField] private float countdownDuration;
        [SerializeField] private bool UseCountdownTrigger;
        private float countdownRemaining;
        private bool countdownActive;
        private Coroutine countdownCoroutine;

        private void Start()
        {
            SetupScene();
            StartCountdown();
        }
        
        public void SetupScene()
        {
            var loaded = Resources.LoadAll<ScriptableObject>("");
            var monobehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<ISceneSetupCallbackReceiver>().ToArray();
            var scriptables = loaded.OfType<ISceneSetupCallbackReceiver>().ToArray();
            
            var receivers = monobehaviours.Concat(scriptables).ToArray();
            
            foreach (var receiver in receivers)
            {
                receiver.OnSceneSetup();
            }
        }

        private void StartCountdown()
        {
            if (!UseCountdownTrigger)
                return;
            
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            countdownActive = true;
            countdownRemaining = countdownDuration;
            while (countdownRemaining > 0f)
            {
                yield return null;
                countdownRemaining -= Time.deltaTime;
            }
            countdownRemaining = 0f;
            countdownActive = false;
            
            SetupScene();
        }

        private void OnGUI()
        {
            if(GUI.Button(new Rect(0, 120,130,25), "Reset Scene", GuiStyleSettings.GetSkin().GetStyle("button")))
            {
                StartCountdown();
            }
        }
    }
}