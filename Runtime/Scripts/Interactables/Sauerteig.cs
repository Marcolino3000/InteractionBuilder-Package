using System;
using System.Collections;
using Nodes.Decorator;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Sauerteig : MonoBehaviour
    {
        public AwarenessLevel awarenessLevel = AwarenessLevel.NotSet;
        public int Activity = 0;
        
        [SerializeField] private DialogTreeRunner treeRunner;
        [SerializeField] private SauerteigStatusDisplay statusDisplay;
        [SerializeField] private DialogOptionNode unlockDialogOption;
        [SerializeField] private bool isUnlocked;
        
        private SphereCollider radarCollider;
         
        private void Awake()
        {
            radarCollider = GetComponent<SphereCollider>();
            treeRunner.DialogNodeSelected += HandleDialogNodeSelected;
        }

        private void HandleDialogNodeSelected(DialogOptionNode node)
        {
            if (!isUnlocked)
                CheckIfSauerteigsGetsUnlocked(node);
            else
                return;
            
            SetActivityBasedOnDialog(node);
        }

        private void CheckIfSauerteigsGetsUnlocked(DialogOptionNode node)
        {
            if (node != unlockDialogOption)
                return;
            
            isUnlocked = true;
            statusDisplay.SetStatusSprite(awarenessLevel);
        }

        private void SetActivityBasedOnDialog(DialogOptionNode node)
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
            if(!isUnlocked)
                return;
            
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
                case 0:
                    awarenessLevel = AwarenessLevel.NotSet;
                    break;
                case 1 or 2:   
                    awarenessLevel = AwarenessLevel.Basic;
                    break;
                case >= 3 and <= 6:
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
            GUILayout.BeginArea(new  Rect(0, 80,300 ,300));
            
            GUI.Label(new Rect(0, 120, 200, 20), $"Activity: {Activity} | Level: {awarenessLevel}");
            
            if(GUI.Button(new Rect(0, 90, 100, 20), "Reset Sauerteig", GUI.skin.button))
            {
                SetActivity(-Activity);
            }
            
            GUILayout.EndArea();
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