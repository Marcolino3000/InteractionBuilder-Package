using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionData")]
    public class InteractionData : WorldStateOwner
    {
        public List<Reaction> Reactions;
        public int Count;
        public int Threshold;
        public bool OneTimeUse;
        
        [SerializeField] private Reaction successReaction;
        [SerializeField] private Reaction failureReaction;

        public override StateData State => new InteractionStateData { Owner = this, ThresholdReached = ThresholdReached };
        public bool ThresholdReached;

        public void HandleInteraction(bool succeeded)
        {
            Count++;
            SetThresholdBool();

            switch (succeeded)
            {
                case true when successReaction != null:
                    successReaction.Execute();
                    break;
                case false when failureReaction != null:
                    failureReaction.Execute();
                    break;
                case true when failureReaction == null || successReaction == null:
                    Debug.LogWarning($"No reaction assigned for {(succeeded ? "success" : "failure")} in interaction {name}");
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

        public class InteractionStateData : StateData
        {
            public bool ThresholdReached;
        }

        public override void SetState(StateData state)
        {
            if (state is InteractionStateData stateData)
            {
                Count = stateData.ThresholdReached ? Threshold : 0;
                ThresholdReached = stateData.ThresholdReached;
            }
                
        }
    }
}

