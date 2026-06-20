using Nodes.Decorator;
using Runtime.Scripts.Core;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class InteractionStarter : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private Interactable currentInteractable;
        [SerializeField] private bool isMovingToInteractable;
        [SerializeField] private bool isDialogRunning;
        [SerializeField] private DialogOptionNode currentDialogOption;
        
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

            DialogTreeRunner.DialogNodeSelected += HandleDialogNodeSelected;
            DialogTreeRunner.OnDialogRunningStatusChanged += HandleDialogRunningStatusChanged;
        }

        private void OnDestroy()
        {
            DialogTreeRunner.DialogNodeSelected -= HandleDialogNodeSelected;
            DialogTreeRunner.OnDialogRunningStatusChanged -= HandleDialogRunningStatusChanged;
        }

        private void HandleDialogNodeSelected(DialogOptionNode node)
        {
            currentDialogOption = node;
        }

        private void HandleDialogRunningStatusChanged(bool isRunning, DialogTree tree)
        {
            isDialogRunning = isRunning;

            if (!isRunning)
                currentDialogOption = null;
        }

        // Self-talk is the only dialog the player may walk away from: clicking to move or to start a
        // new interaction is allowed to replace it. During any other dialog the player stays locked in.
        private bool CanInterruptCurrentDialog()
        {
            if (!isDialogRunning)
                return true;

            return currentDialogOption is PlayerDialogOption { Type: AnswerType.SelfTalk };
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
            if (!CanInterruptCurrentDialog())
                return;

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
            if (!CanInterruptCurrentDialog())
                return;

            currentInteractable = null;
            isMovingToInteractable = false;
            moveByClick.HandleMouseClick(false, targetPosition);
        }
    }
}