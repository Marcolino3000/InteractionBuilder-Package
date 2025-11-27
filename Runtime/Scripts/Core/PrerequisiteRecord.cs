using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Runtime.Scripts.Interactables;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [Serializable]
    public record PrerequisiteRecord ()
    {
        public InteractionData InteractionToExecute;
        public DialogOptionNode DialogOptionToUnlock;
        public InteractionTriggerVia TriggerType;
        public InteractableState TriggeringInteractable;
        public List<StateWithSettings> Conditions;
        
        public InteractionData Execute()
        {
            if (InteractionToExecute == null && DialogOptionToUnlock == null)
            {
                Debug.LogWarning("Interaction and DialogOption are null! Returning early.");
                return null;
            }
            
            ExecutionStatus status = CheckConditions();     
            
            if (InteractionToExecute != null)
                if (!TryExecuteInteraction(status))
                    return null;
            
            if(DialogOptionToUnlock != null)
            {
                if(status == ExecutionStatus.Succeeded)
                {
                    DialogOptionToUnlock.IsAvailable = true;
                }
            }
            
            return InteractionToExecute;
        }

        private bool TryExecuteInteraction(ExecutionStatus status)
        {
            if(InteractionToExecute.OneTimeUse && InteractionToExecute.Count > 0)
                return false;
            
            switch (status)
            {
                case ExecutionStatus.Cancelled:
                    return false;
                case ExecutionStatus.Failed:
                    InteractionToExecute?.HandleInteraction(false);
                    return true;
                case ExecutionStatus.Succeeded:
                    InteractionToExecute?.HandleInteraction(true);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        //Hard conditions have to be on top of the list otherwise they won't have any effect as the status will
        //be set to Failed instead of Cancelled if a soft condition is checked and fails first.
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
        [SerializeReference] public WorldStateOwner.StateData RecordedState;
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