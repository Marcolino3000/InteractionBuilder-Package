using System;
using System.Linq;
using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class SceneSetup : MonoBehaviour
    {
        private void Awake()
        {
            SetupScene();
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

        private void OnGUI()
        {
            if(GUILayout.Button("Reset Scene"))
            {
                SetupScene();
            }
        }
    }
}