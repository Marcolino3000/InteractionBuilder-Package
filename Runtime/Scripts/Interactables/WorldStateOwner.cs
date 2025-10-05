using System;
using Runtime.Scripts.Core;
using Runtime.Scripts.Utility;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class WorldStateOwner : ScriptableObject 
    {
        public virtual StateData State => new() { Owner = this, Name = name };
        
        [InspectorButton("SaveStateToCurrentScene")]
        public bool _SaveStateToCurrentScene; 
        
        [HideInInspector][AutoAssign][SerializeField] protected SceneStateSaver _sceneStateSaver;
        
        public virtual void SetState(StateData state)
        {
            if (state == null)
            {
                Debug.LogWarning("State to set was null");
                return;
            }
                
        }
        
        [Serializable]
        public class StateData
        {
            public WorldStateOwner Owner;
            public string Name;
        }
        
        public void SaveStateToCurrentScene()
        {
            _sceneStateSaver.SaveCurrentState( this, State);
        }
    }
}