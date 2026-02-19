using System;
using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionsCounter")]
    public class InteractionsCounter : ScriptableObject
    {
        public event Action<InteractionTriggerVia, InteractableState> OnThresholdReached; 
        
        [SerializeField] private InteractableState interactionBeforePaul;
        [SerializeField] private bool interactionWasTriggered;
        [SerializeField] private int interactionsCountBeforePaul;
        [SerializeField] private int currentInteractionsCount;

        // [SerializeField] private InteractionData interactionToExecute;

        private void FindInteractablesInScene()
        {
            var interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (var interactable in interactables)
            {
                interactable.OnInteractionStarted += HandleInteractionStarted;
            }
        }

        private void HandleInteractionStarted(InteractionTriggerVia via, InteractableState state)
        {
            Debug.Log($"Interaction started via {via} on {state}");
            
            currentInteractionsCount++;

            if(state == interactionBeforePaul)
                interactionWasTriggered = true;

            if (interactionWasTriggered && currentInteractionsCount >= interactionsCountBeforePaul)
            {
                OnThresholdReached?.Invoke(InteractionTriggerVia.ThresholdReached, interactionBeforePaul);
                Debug.Log("Counter: Threshold reached");
            }
        }

        public void Setup()
        {
            FindInteractablesInScene();
            currentInteractionsCount = 0;
            interactionWasTriggered = false;
        }
    }
}