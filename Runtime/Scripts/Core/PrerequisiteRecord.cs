using System;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using Sirenix.Utilities;
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
        // public WorldStateOwner.StateData InteractableData;
        // public bool IsHardCondition;
        public List<StateWithSettings> Conditions;

        public void TryExecuteInteraction()
        {
            if(InteractionToExecute.OneTimeUse && InteractionToExecute.Count > 0)
                return;

            ExecutionStatus status = CheckConditions(); 
            
            if(status == ExecutionStatus.Failed)
                InteractionToExecute?.HandleInteraction(false);
            if(status == ExecutionStatus.Succeeded)
                InteractionToExecute?.HandleInteraction(true);
        }

        // private bool CompareStates(InteractionData interaction)
        // {
        //     if (interaction == null)
        //         return true;
        //  
        //     return inter
        // } 

        private ExecutionStatus CheckConditions()
        {
            if (Conditions.IsNullOrEmpty())
                return ExecutionStatus.Succeeded;

            foreach (var condition in Conditions)
            {
                if (condition.RecordedState.Owner.CurrentState != condition.RecordedState)
                {
                    if (condition.IsHardCondition)
                        return ExecutionStatus.Cancelled;
                    return ExecutionStatus.Failed;
                }
            }
            
            return ExecutionStatus.Succeeded;
        }
    }

    [Serializable]
    public class StateWithSettings
    {
        public WorldStateOwner.StateData RecordedState;
        public bool IsHardCondition;
    }

    [Serializable]
    public enum ExecutionStatus
    {
        Succeeded,
        Failed,
        Cancelled
    }
}