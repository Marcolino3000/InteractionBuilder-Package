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
        public event Action<DialogTree> OnSetDialogTree;

        public DialogTree DialogTree;
        public InteractableState Interactable;
        public InteractableState ObjectToMoveIn;
        public InteractableState ObjectToMoveOut;
        public bool CancelCurrentDialog;
        
        public void Execute()
        {
            if (CancelCurrentDialog)
            {
                Debug.Log("Cancel Dialog called");
                OnStopDialog?.Invoke();
            }

            if(DialogTree != null)
            {
                OnSetDialogTree?.Invoke(DialogTree);
                OnStopDialog?.Invoke();
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
        }
    }
}