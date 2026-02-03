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

        public DialogTree DialogTree;
        public InteractableState Interactable;
        public InteractableState ObjectToMoveIn;
        public InteractableState ObjectToMoveOut;
        public bool CancelCurrentDialog;
        public string DebugMessage;
        public ScriptedSequence ScriptedSequence;


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
                ObjectToMoveIn.Interactable.transform.position += new Vector3(0,5,0);
            }

            if (ObjectToMoveOut != null)
            {
                ObjectToMoveOut.Interactable.transform.position += new Vector3(0,-5,0);
            }

            if (ScriptedSequence != null)
            {
                OnStartSequence?.Invoke(ScriptedSequence.waypoints, SequenceFinishedCallback);
            }
            
            if(DialogTree == null && ScriptedSequence == null)
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