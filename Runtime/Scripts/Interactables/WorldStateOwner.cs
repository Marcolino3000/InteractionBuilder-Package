using System;
using Runtime.Scripts.Core;
using Runtime.Scripts.Utility;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class WorldStateOwner : ScriptableObject 
    {
        public virtual StateData CurrentState => new() { Owner = this};
        
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
        public record StateData
        {
            public WorldStateOwner Owner;
            // public string Name;
        }
        
        public void SaveStateToCurrentScene()
        {
            _sceneStateSaver.SaveCurrentState( this, CurrentState);
        }
    }
}