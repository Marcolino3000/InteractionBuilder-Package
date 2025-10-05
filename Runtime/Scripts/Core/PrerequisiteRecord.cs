using System;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [Serializable]
    public record PrerequisiteRecord()
    {
        //Interaction to execute
        public InteractionData InteractionToExecute;
        //Trigger
        public InteractionTriggerVia TriggerType;
        public InteractableState TriggeringInteractable;
        //Conditions
        public InteractableState InteractableData;
        public string InteractableState;
        public InteractionData InteractionData;
        public bool IsHardCondition;
        public string InteractionState;

        public bool TryExecuteInteraction(InteractableState triggeringInteractable)
        {
            if(InteractionToExecute.OneTimeUse && InteractionToExecute.Count > 0)
                return false;
            
            bool conditionsMet = true;
            
            if(!CompareStates(InteractableState, InteractableData))
                conditionsMet = false;
            
            if(!CompareStates(InteractionState, InteractionData))
            {
                conditionsMet = false;
                
                if(IsHardCondition)
                    return false;
            }
            
            InteractionToExecute?.HandleInteraction(conditionsMet);
            // triggeringInteractable.HandleInteractionCallback(conditionsMet);
            
            return conditionsMet;
        }

        private bool CompareStates(string recordedState, InteractionData interaction)
        {
            if (interaction == null)
                return true;
            
            var currentState = JsonUtility.ToJson(interaction.State, true);
            return recordedState== currentState;
        }

        private bool CompareStates(string recordedState, InteractableState interactable)
        {
            if (interactable == null)
                return true;
            
            var currentState = JsonUtility.ToJson(interactable.State, true);
            return recordedState == currentState;
        }
    }
}