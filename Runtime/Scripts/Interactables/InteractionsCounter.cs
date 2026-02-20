using System;
using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionsCounter")]
    public class InteractionsCounter : ScriptableObject
    {
        public event Action<InteractionTriggerVia, InteractableState> OnThresholdReached;

        [SerializeField] private bool isActive;
        [SerializeField] private InteractableState requiredInteraction;
        [SerializeField] private bool interactionWasTriggered;
        [SerializeField] private int interactionsCountBeforePaul;
        [SerializeField] private int currentInteractionsCount;

        [SerializeField] private InteractionData startCountingInteraction;
        [SerializeField] private InteractionData stopCountingInteraction;

        private void FindInteractablesInScene()
        {
            var interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
            foreach (var interactable in interactables)
            {
                var capturedInteractable = interactable;
                interactable.OnInteractionStarted += HandleInteractionStarted;
                interactable.OnInteractionSuccessful += () => HandleSuccessfulInteraction(capturedInteractable.Data);
            }
        }

        private void HandleInteractionStarted(InteractionTriggerVia via, InteractableState state)
        {
 
        }

        private void HandleSuccessfulInteraction(InteractableState state)
        {
            if (!isActive)
                return;
            
            if (startCountingInteraction != null && !startCountingInteraction.ThresholdReached)
                return;
            
            if (stopCountingInteraction != null && stopCountingInteraction.ThresholdReached)
                return;
            currentInteractionsCount++;

            if(requiredInteraction == null || state == requiredInteraction)
                interactionWasTriggered = true;

            if (interactionWasTriggered && currentInteractionsCount >= interactionsCountBeforePaul)
            {
                OnThresholdReached?.Invoke(InteractionTriggerVia.ThresholdReached, requiredInteraction);
                Debug.Log("Counter: Threshold reached");
            }
        }

        public void Setup()
        {
            FindInteractablesInScene();
            currentInteractionsCount = 0;
            interactionWasTriggered = false;
        }

        public void SetActive(bool toggle)
        {
            isActive = toggle;
        }
    }
}