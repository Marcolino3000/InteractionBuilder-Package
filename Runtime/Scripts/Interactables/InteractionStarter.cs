using Runtime.Scripts.Core;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class InteractionStarter : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private Interactable currentInteractable;
        [SerializeField] private bool isMovingToInteractable;
        
        [Header("Settings")]
        [SerializeField] private bool moveToInteractableOnClick;
        
        [Header("References")]
        [SerializeField] private Raycaster raycaster;
        [SerializeField] private MoveByClick moveByClick;
        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private InteractionHandler interactionHandler;


        private void Awake()
        {
            raycaster.OnInteractableClicked += HandleInteractableClicked;
            raycaster.OnGroundClicked += HandleGroundClicked;
            moveByClick.OnNavMeshMovementEnded += HandleMovementEnded;
            interactionHandler.OnPrerequisiteReady += HandlePrerequisiteReady;
        }

        private void HandlePrerequisiteReady(PrerequisiteRecord prereq)
        {
            if (isMovingToInteractable)
            {
                currentInteractable = null;
                isMovingToInteractable = false;
            }
            
            prereq.Execute();
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
            
            isMovingToInteractable = false;
            currentInteractable = null;
        }

        private void HandleInteractableClicked(Interactable interactable)
        {
            currentInteractable = interactable;
            isMovingToInteractable = true;

            if(currentInteractable == null)
            {
                Debug.LogError("HandleInteractableClicked was called but interactable was null. Returning early");
                return;
            }
        
            moveByClick.HandleMouseClick(true, interactable.transform.position);
        }

        private void HandleGroundClicked(Vector3 targetPosition)
        {
            currentInteractable = null;
            isMovingToInteractable = false;
            moveByClick.HandleMouseClick(false, targetPosition);
        }
    }
}