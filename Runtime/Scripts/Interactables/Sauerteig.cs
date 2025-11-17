using System;
using System.Collections;
using Nodes.Decorator;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Sauerteig : MonoBehaviour
    {
        public AwarenessLevel awarenessLevel;
        public int Activity = 1;
        
        [SerializeField] private DialogTreeRunner treeRunner;
        
        private SphereCollider radarCollider;
        
         
        private void Awake()
        {
            radarCollider = GetComponent<SphereCollider>();
            treeRunner.DialogNodeSelected += HandleDialogNodeSelected;
        }

        private void HandleDialogNodeSelected(DialogOptionNode node)
        {
            if (node is PlayerDialogOption playerOption)
            {
                switch (playerOption.Type)
                {
                    case AnswerType.SmallTalk:
                        Activity -= 1;
                        break;
                    case AnswerType.DeepTalk:
                        Activity -= 2;
                        break;
                    case AnswerType.TrashTalk:
                        Activity += 1;
                        break;
                    case AnswerType.BusinessTalk:
                        Activity -= 3;
                        break;
                }
            }
        }


        public void ActivateRadar()
        {
            radarCollider.enabled = true;
            
            StartCoroutine(DeactivateRadarNextFrame());
        }

        private IEnumerator DeactivateRadarNextFrame()
        {
            yield return new WaitForSeconds(0.1f);
            
            radarCollider.enabled = false;
            
            if(awarenessLevel != AwarenessLevel.Basic)
                SetActivity(-1);
        }

        public void HandleInteractableDiscovered(InteractableState interactable)
        {
            switch (interactable.AwarenessLevel)
            {
                case AwarenessLevel.Basic:
                    SetActivity(2);
                    break;
                case AwarenessLevel.Super:
                    SetActivity(5);
                    break;
            }
        }

        private void SetActivity(int activityChange)
        {
            Activity += activityChange;
            
            if(Activity < 1)
                Activity = 1;
            
            switch (Activity)
            {
                case 0 or 1:   
                    awarenessLevel = AwarenessLevel.Basic;
                    break;
                case >= 2:
                    awarenessLevel = AwarenessLevel.Super;
                    break;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(1400, 550, 200, 30), $"Activity: {Activity} | Level: {awarenessLevel}");
        }
    }
    
    public enum AwarenessLevel
    {
        NotSet,
        Basic,
        Super
    }
}