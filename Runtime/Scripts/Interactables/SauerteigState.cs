using System;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Interactable/SauerteigState")]
    public class SauerteigState : InteractableState
    {
        [Header("Debug")] 
        public AwarenessLevel CurrentLevel;
        
        public override StateData CurrentState => new SauerteigStateData { Owner = this, AwarenessLevel = CurrentLevel };
        
        // public override void HandleInteraction()
        // {
        //     base.HandleInteraction();
        // }
        
        [Serializable]
        private record SauerteigStateData : StateData
        {
            public AwarenessLevel AwarenessLevel;
        }

        public override void SetState(StateData state)
        {
            if (state is SauerteigStateData stateData)
            {
                CurrentLevel = stateData.AwarenessLevel;
            }
        }
    }
    
}