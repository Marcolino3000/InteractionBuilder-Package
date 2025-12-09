using System;
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
        public event Action<Waypoints> OnStartSequence;

        public DialogTree DialogTree;
        public InteractableState Interactable;
        public InteractableState ObjectToMoveIn;
        public InteractableState ObjectToMoveOut;
        public bool CancelCurrentDialog;
        public string DebugMessage;
        public Waypoints Waypoints;


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
                ObjectToMoveIn.Interactable.transform.position += new Vector3(14,0,0);
            }

            if (ObjectToMoveOut != null)
            {
                ObjectToMoveOut.Interactable.transform.position += new Vector3(-14,0,0);
            }

            if (Waypoints != null)
            {
                OnStartSequence?.Invoke(Waypoints);
            }
            
            if(DialogTree == null)
                OnReactionFinished?.Invoke(true);
        }

        private void DialogFinishedCallback(bool ranToCompletion)
        {
            Debug.Log(name + " Dialog Finished Callback, ranToCompletion: " + ranToCompletion);
            OnReactionFinished?.Invoke(ranToCompletion);
        }

        public void SetWaypoints(Waypoints waypoints)
        {
            Waypoints = waypoints;
        }
    }
}