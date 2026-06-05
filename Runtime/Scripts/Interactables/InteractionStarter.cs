using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class InteractionStarter : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private Interactable currentInteractable;
        
        [Header("Settings")]
        [SerializeField] private bool moveToInteractableOnClick;
        
        [Header("References")]
        [SerializeField] private Raycaster raycaster;
        [SerializeField] private MoveByClick moveByClick;
        [SerializeField] private Sauerteig sauerteig;


        private void Awake()
        {
            raycaster.OnClick += HandleClick;
            moveByClick.OnNavMeshMovementEnded += HandleMovementEnded;
        }

        private void HandleMovementEnded()
        {
            if (currentInteractable == null)
                return;
            
            if(currentInteractable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                currentInteractable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, currentInteractable.Data);

            if (currentInteractable.Data.AwarenessLevel != AwarenessLevel.NotSet && !sauerteig.IsUnlocked)
            {
                currentInteractable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, currentInteractable.Data);
            }

            else if(currentInteractable.Data.AwarenessLevel != AwarenessLevel.NotSet && sauerteig.awarenessLevel >= currentInteractable.Data.AwarenessLevel)
            {
                currentInteractable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, currentInteractable.Data);

                sauerteig.HandleInteractableDiscovered(currentInteractable.Data);

                if(currentInteractable.MarkAsFoundWhenClicked)
                    currentInteractable.Found = true;
            }
        }

        private void HandleClick(bool isInteraction, Interactable interactable)
        {
            currentInteractable = interactable;

            moveByClick.HandleMouseClick(isInteraction);
        }
    }
}