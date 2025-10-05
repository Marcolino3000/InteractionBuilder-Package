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
        public event Action<DialogTree> OnSetDialogTree;

        public DialogTree DialogTree;
        public InteractableState Interactable;
        public GameObject ObjectToMoveIn;
        public GameObject ObjectToMoveOut;

        public void Execute()
        {
            if(DialogTree != null)
            {
                OnSetDialogTree?.Invoke(DialogTree);
                OnStartDialog?.Invoke();
            }
            
            if(Interactable != null)
            {
                Interactable?.HandleInteraction();
            }

            if (ObjectToMoveIn != null)
            {
                ObjectToMoveIn.transform.position += new Vector3(0,2,0);
            }
            
            if (ObjectToMoveOut != null)
            {
                ObjectToMoveOut.transform.position += new Vector3(0,-2,0);
            }
        }
    }
}