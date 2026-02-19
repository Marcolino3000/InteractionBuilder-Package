using System;
using System.Collections.Generic;
using Core;
using Runtime.Scripts.Interactables;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Reaction")]
    public class Reaction : ScriptableObject, IDialogStarter, IDialogTreeSetter
    {
        public event Action OnStartDialog;
        public event Action OnStopDialog;
        public event Action<DialogTree, Action<bool>> OnSetDialogTree;
        public event Action<bool> OnReactionFinished;
        public event Action<List<Waypoint>, Action> OnStartSequence;
        public event Action<InteractableState, Action> OnShowInteractable;

        public DialogTree DialogTree;
        public InteractableState Interactable;
        public InteractableState ObjectToMoveIn;
        public InteractableState ObjectToMoveOut;
        public InteractableState ObjectToMove;
        public Vector3 TargetPosition;
        public bool CancelCurrentDialog;
        public string DebugMessage;
        public ScriptedSequence ScriptedSequence;
        public InteractableState InteractableToShow;

        public void Execute()
        {
            if (!string.IsNullOrEmpty(DebugMessage))
            {
                Debug.Log(name + " " + DebugMessage);
            }
            
            if (CancelCurrentDialog)
            {
                Debug.Log("Cancel Dialog called");
                OnStopDialog?.Invoke();
            }

            if(DialogTree != null)
            {
                OnStopDialog?.Invoke();
                OnSetDialogTree?.Invoke(DialogTree, DialogFinishedCallback);
                OnStartDialog?.Invoke();
            }

            if(Interactable != null)
            {
                Interactable?.HandleInteraction();
            }

            if (ObjectToMoveIn != null)
            {
                if (ObjectToMoveIn.Interactable.Spawned) return;
                
                ObjectToMoveIn.Interactable.transform.position += new Vector3(0,30,0);
                ObjectToMoveIn.Interactable.Spawned = true;
            }

            if (ObjectToMoveOut != null)
            {
                if (!ObjectToMoveIn.Interactable.Spawned) return;
                
                ObjectToMoveOut.Interactable.transform.position += new Vector3(0,-30,0);
                ObjectToMoveIn.Interactable.Spawned = false;
            }
            
            if (ObjectToMove != null)
            {
                ObjectToMove.Interactable.transform.position += TargetPosition;
            }

            if (ScriptedSequence != null)
            {
                OnStartSequence?.Invoke(ScriptedSequence.waypoints, SequenceFinishedCallback);
            }
            
            if(DialogTree == null && ScriptedSequence == null)
                OnReactionFinished?.Invoke(true);

            if (InteractableToShow != null)
            {
                OnShowInteractable?.Invoke(InteractableToShow, ShowInteractableCallback);
            }
        }

        private void ShowInteractableCallback()
        {
            Debug.Log(name + " Show Interactable finished");
            OnReactionFinished?.Invoke(true);
        }

        private void SequenceFinishedCallback()
        {
            Debug.Log(name + " Sequence Finished Callback");
            OnReactionFinished?.Invoke(true);
        }

        private void DialogFinishedCallback(bool ranToCompletion)
        {
            Debug.Log(name + " Dialog Finished Callback, ranToCompletion: " + ranToCompletion);
            OnReactionFinished?.Invoke(ranToCompletion);
        }
        
        
    }
}