using System;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using Runtime.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Object/Scene State Saver")]
    public class SceneStateSaver :  ScriptableObject
    {
        [InspectorButton("LoadCurrentSceneStates")]
        public bool LoadState;
        
        [SerializeReference] private List<WorldStateOwner.StateData> states;

        public void LoadCurrentSceneStates()
        {
            foreach (var state in states)
            {
                state.Owner.SetState(state);
            }
        }
        
        public void SaveCurrentState(WorldStateOwner owner, WorldStateOwner.StateData state)
        {
            if (states == null)
                states = new List<WorldStateOwner.StateData>();
            
            states.RemoveAll(s => s.Owner == owner);
            
            states.Add(state);
        }
        
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode)
                {
                    LoadCurrentSceneStates();
                }
            };
        }
    }
}