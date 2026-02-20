using System;
using System.Collections.Generic;
using NUnit.Framework;
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
        // [SerializeField] private Reaction requiredReaction;
        [SerializeField] private bool interactionWasTriggered;
        [SerializeField] private int interactionsCountBeforePaul;
        [SerializeField] private int currentInteractionsCount;
        [SerializeField] private Toggleable unlockToggle;

        [SerializeField] private InteractionData startCountingInteraction;
        [SerializeField] private InteractionData stopCountingInteraction;
        [SerializeField] private List<Reaction> countedReactions;

        // private void FindInteractablesInScene()
        // {
        //     var interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        //     foreach (var interactable in interactables)
        //     {
        //         var capturedInteractable = interactable;
        //         interactable.OnInteractionStarted += HandleInteractionStarted;
        //         interactable.OnInteractionSuccessful += () => HandleSuccessfulInteraction(capturedInteractable.Data);
        //     }
        // }
        //
        // private void HandleInteractionStarted(InteractionTriggerVia via, InteractableState state)
        // {
        //
        // }

        // private void HandleSuccessfulInteraction(InteractableState state)
        // {
        //     if (!isActive)
        //         return;
        //     
        //     if (startCountingInteraction != null && !startCountingInteraction.ThresholdReached)
        //         return;
        //     
        //     if (stopCountingInteraction != null && stopCountingInteraction.ThresholdReached)
        //         return;
        //     currentInteractionsCount++;
        //
        //     if(requiredInteraction == null || state == requiredInteraction)
        //         interactionWasTriggered = true;
        //
        //     if (interactionWasTriggered && currentInteractionsCount >= interactionsCountBeforePaul)
        //     {
        //         OnThresholdReached?.Invoke(InteractionTriggerVia.ThresholdReached, requiredInteraction);
        //         Debug.Log("Counter: Threshold reached");
        //     }
        // }

        public void Setup()
        {
            // FindInteractablesInScene();
            FindReactions();
            currentInteractionsCount = 0;
            interactionWasTriggered = false;
            countedReactions = new List<Reaction>();
        }

        private void FindReactions()
        {
            var reactions = Resources.FindObjectsOfTypeAll<Core.Reaction>();
            foreach (var reaction in reactions)
            {
                var capturedReaction = reaction;
                reaction.OnReactionFinished += (completed) => HandleReactionFinished(capturedReaction);
            }
        }

        private void HandleReactionFinished(Reaction reaction)
        {
            if (!isActive)
                return;

            if (countedReactions.Contains(reaction))
                return;
            
            if (startCountingInteraction != null && !startCountingInteraction.ThresholdReached)
                return;
            
            if (stopCountingInteraction != null && stopCountingInteraction.ThresholdReached)
                return;
            
            currentInteractionsCount++;
            countedReactions.Add(reaction);

            if(requiredInteraction == null || reaction.Interactable == requiredInteraction)
                interactionWasTriggered = true;

            if (interactionWasTriggered && currentInteractionsCount >= interactionsCountBeforePaul)
            {
                OnThresholdReached?.Invoke(InteractionTriggerVia.ThresholdReached, requiredInteraction);
                unlockToggle.ToggleState = true;
                Debug.Log("Counter: Threshold reached");
            }
        }

        public void SetActive(bool toggle)
        {
            isActive = toggle;
        }
    }
}