using System;
using System.Collections;
using Nodes.Decorator;
using Runtime.Scripts.Animation;
using Runtime.Scripts.PlayerInput;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Sauerteig : MonoBehaviour, IPlayerInputReceiver
    {
        public AwarenessLevel awarenessLevel = AwarenessLevel.Basic;
        public AwarenessLevel levelBeforeUnlock;
        public AwarenessLevel levelAfterUnlock;
        public int Activity = 1;
        public bool IsUnlocked;

        [SerializeField] private DialogTreeRunner treeRunner;
        [SerializeField] private SauerteigStatusDisplay statusDisplay;
        [SerializeField] private DialogOptionNode unlockDialogOption;
        [SerializeField] private DialogOptionNode emotionalReactionDialogOption;
        [SerializeField] private SuperDuperWackler wackler;
        [SerializeField] private GigaGlowManager glowManager;

        private SphereCollider radarCollider;
         
        private void Awake()
        {
            radarCollider = GetComponent<SphereCollider>();
            DialogTreeRunner.DialogNodeSelected += HandleDialogNodeSelected;
            awarenessLevel = levelBeforeUnlock;
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponentInParent<Interactable>();
            
            if(interactable == null) 
                return;

            if (interactable.Data.AwarenessLevel <= AwarenessLevel.Basic)
                return;
            
            if(IsUnlocked)
                wackler.Wiggle(interactable);
        }

        private void HandleDialogNodeSelected(DialogOptionNode node)
        {
            if (!IsUnlocked)
            {
                if (!CheckIfSauerteigsGetsUnlocked(node))
                    return;
            }
            
            CheckForEmotionalReaction(node);
            
            SetActivityBasedOnDialog(node);
        }

        private void CheckForEmotionalReaction(DialogOptionNode node)
        {
            if (node != emotionalReactionDialogOption)
                return;
            
            glowManager.Glow();
            wackler.Wiggle();
        }

        private bool CheckIfSauerteigsGetsUnlocked(DialogOptionNode node)
        {
            if (node != unlockDialogOption)
                return false;
            
            IsUnlocked = true;
            awarenessLevel = levelAfterUnlock;
            statusDisplay.ShowSauerteig();
            statusDisplay.UpdateStatus((int)awarenessLevel);
            
            return true;
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
                if (playerOption.Type == AnswerType.SelfTalk)
                    return;
                
                switch (playerOption.Type)
                {
                    case AnswerType.SmallTalk:
                        SetActivity(-1);
                        break;
                    case AnswerType.DeepTalk:
                        SetActivity(-2);
                        break;
                    case AnswerType.TrashTalk:
                        SetActivity(-1);
                        break;
                }
            }
        }

        public GigaGlowManager GetGlowManager()
        {
            return glowManager;
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
            if(!IsUnlocked)
                return;
            
            switch (interactable.AwarenessLevel)
            {
                case AwarenessLevel.Basic:
                    SetActivity(2);
                    break;
                case AwarenessLevel.Super:
                    SetActivity(4);
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
                case >= 3 and <= 10:
                    awarenessLevel = AwarenessLevel.Super;
                    break;
                case > 10:
                    awarenessLevel = AwarenessLevel.Overflow;
                    break;
            }
            
            if(IsUnlocked)
                statusDisplay.UpdateStatus(Activity);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new  Rect(0, 160,300 ,300));
            
            GUI.Label(new Rect(0, 40, 200, 25), $"Activity: {Activity} | Level: {awarenessLevel}");
            
            if(GUI.Button(new Rect(0, 0, 130, 25), "Reset Sauerteig", GUI.skin.button))
            {
                SetActivity(-Activity);
            }
            
            GUILayout.EndArea();
        }

        public void HandleTrigger()
        {
            throw new NotImplementedException();
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