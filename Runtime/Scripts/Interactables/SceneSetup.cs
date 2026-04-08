using System.Linq;
using Runtime.Scripts.Utility;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class SceneSetup : MonoBehaviour
    {
        [SerializeField] private float countdownDuration;
        [SerializeField] private bool UseCountdownTrigger;
        
        [Header("Settings")]
        [SerializeField] private bool showDebugButtons;
        
        private float countdownRemaining;
        private bool countdownActive;
        private Coroutine countdownCoroutine;

        private void Start()
        {
            SetupScene();
        }

        private void SetupScene()
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

        private void OnGUI()
        {
            if (!showDebugButtons)
                return;
            
            if(GUI.Button(new Rect(0, 120,130,25), "Reset Scene", GuiStyleSettings.GetSkin().GetStyle("button")))
            {
                SetupScene();
            }
        }
    }
}