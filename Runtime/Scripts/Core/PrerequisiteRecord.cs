using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Runtime.Scripts.Interactables;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [Serializable]
    public record PrerequisiteRecord()
    {
        public InteractionData InteractionToExecute;
        public DialogOptionNode DialogOptionToUnlock;
        public InteractionTriggerVia TriggerType;
        public InteractableState TriggeringInteractable;
        public List<StateWithSettings> Conditions;
        
        public void Execute()
        {
            if (InteractionToExecute == null && DialogOptionToUnlock == null)
            {
                Debug.LogWarning("Interaction and DialogOption are null! Returning early.");
                return;
            }
            
            ExecutionStatus status = CheckConditions();     
            
            if (InteractionToExecute != null)
                TryExecuteInteraction(status);
            
            if(DialogOptionToUnlock != null)
            {
                if(status == ExecutionStatus.Succeeded)
                {
                    DialogOptionToUnlock.IsAvailable = true;
                }
            }
        }

        private void TryExecuteInteraction(ExecutionStatus status)
        {
            if(InteractionToExecute.OneTimeUse && InteractionToExecute.Count > 0)
                return;
            
            if(status == ExecutionStatus.Failed)
                InteractionToExecute?.HandleInteraction(false);
            if(status == ExecutionStatus.Succeeded)
            {
                InteractionToExecute?.HandleInteraction(true);
            }
        }

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