using System;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionData")]
    public class InteractionData : WorldStateOwner
    {
        public int Count;
        public int Threshold;
        public bool OneTimeUse;
        
        [SerializeField] public Reaction successReaction;
        [SerializeField] public Reaction failureReaction;

        public override StateData CurrentState => new InteractionStateData { Owner = this, ThresholdReached = ThresholdReached, IsRunning = IsRunning };
        public bool ThresholdReached;
        public bool IsRunning;
        public bool IsActive = true;
        
        public void HandleInteractionStop()
        {
            if(!IsActive)
            {
                Debug.LogWarning("Interaction was stopped even though is was not active, this should not happen!");
                return;
            }
            
            IsRunning = false;
            Count++;
            SetThresholdBool();
        }

        private void HandleInteractionStop(bool ranToCompletion)
        {
            if(!IsActive)
            {
                Debug.LogWarning("Interaction was stopped even though is was not active, this should not happen");
                return;
            }
            
            IsRunning = false;
            Count++;
            SetThresholdBool();
        }
        
        public void HandleInteractionStart(bool succeeded = false)
        {
            // Count++;
            // SetThresholdBool();

            if(!IsActive) 
                return;
            
            switch (succeeded)
            {
                case true when successReaction != null:
                    // successReaction.OnReactionFinished += HandleInteractionStop;
                    IsRunning = true;
                    successReaction.Execute();
                    break;
                case false when failureReaction != null:
                    // failureReaction.OnReactionFinished += HandleInteractionStop;
                    IsRunning = true;
                    failureReaction.Execute();
                    break;
                case true when successReaction == null:
                    Debug.LogWarning($"No reaction assigned for success in interaction {name}");
                    break;
                case false when failureReaction == null:
                    Debug.LogWarning($"No reaction assigned for failure in interaction {name}");
                    break;
            }
        }

        private void SetThresholdBool()
        {
            ThresholdReached = Count >= Threshold;
        }

        private void OnValidate()
        {
            SetThresholdBool();
        }

        private void OnEnable()
        {
            if(successReaction != null)
                successReaction.OnReactionFinished += HandleInteractionStop;
            if(failureReaction != null)
                failureReaction.OnReactionFinished += HandleInteractionStop;
        }

        [Serializable]
        public record InteractionStateData : StateData
        {
            public bool ThresholdReached;
            public bool IsRunning;
        }

        public override void SetState(StateData state)
        {
            if (state is InteractionStateData stateData)
            {
                Count = stateData.ThresholdReached ? Threshold : 0;
                ThresholdReached = stateData.ThresholdReached;
                IsRunning = stateData.IsRunning;
            }
                
        }
    }
}

