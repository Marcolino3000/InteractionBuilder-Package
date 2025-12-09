using System;
using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractableState")]
    public class InteractableState : WorldStateOwner
    {
        public event Action OnInteractionFeedback;
        
        public Interactable Interactable;

        public Texture2D Sprite;
        public AwarenessLevel AwarenessLevel;
                
        public virtual void HandleInteraction()
        {
            OnInteractionFeedback?.Invoke();
        }

        public override void SetState(StateData state)
        {
            base.SetState(state);

            if (state is StateData stateData)
            {
                // Name = stateData.Name;
            }
                
        }
        
        public void SetInteractable(Interactable interactable)
        {
            Interactable = interactable;
        }
    }
    
  
}