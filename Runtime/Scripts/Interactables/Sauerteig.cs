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
        [SerializeField] private SauerteigStatusDisplay statusDisplay;
        
        private SphereCollider radarCollider;
         
        private void Awake()
        {
            radarCollider = GetComponent<SphereCollider>();
            treeRunner.DialogNodeSelected += HandleDialogNodeSelected;
        }

        private void HandleDialogNodeSelected(DialogOptionNode node)
        {
            if (node.Blackboard == null || node.Blackboard.CharacterData == null)
            {
                Debug.LogWarning("Node-Blackboard or CharacterData was null. Bonding-status was not checked.");
            }

            else
            {
                if(node.Blackboard.CharacterData.BondedWithPlayer)
                    return;    
            }
            
            if (node is PlayerDialogOption playerOption)
            {
                switch (playerOption.Type)
                {
                    case AnswerType.SmallTalk:
                        SetActivity(-1);
                        break;
                    case AnswerType.DeepTalk:
                        SetActivity(-2);
                        break;
                    case AnswerType.TrashTalk:
                        SetActivity(1);
                        break;
                    case AnswerType.BusinessTalk:
                        SetActivity(-3);
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
                case >= 2 and <= 6:
                    awarenessLevel = AwarenessLevel.Super;
                    break;
                case > 6:
                    awarenessLevel = AwarenessLevel.Overflow;
                    break;
            }
            
            statusDisplay.SetStatusSprite(awarenessLevel);
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(50, 100, 200, 30), $"Activity: {Activity} | Level: {awarenessLevel}");
        }
    }
    
    public enum AwarenessLevel
    {
        NotSet,
        Basic,
        Super,
        Overflow
    }
}