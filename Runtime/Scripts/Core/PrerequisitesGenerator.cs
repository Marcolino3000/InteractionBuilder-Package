using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Runtime.Scripts.Interactables;
using Runtime.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/PrerequisitesGenerator")]
    public class PrerequisitesGenerator : ScriptableObject
    {
        [Header("Interaction to execute")]
        [SerializeField] private InteractionData InteractionToExecute;
        [SerializeField] private DialogOptionNode DialogOptionToUnlock;

        [Header("Prerequisites for interaction")]
        [Header("Trigger Settings")]
        [SerializeField] private bool IsHighPriority;
        [SerializeField] private InteractionTriggerVia TriggerType;
        [SerializeField] private InteractableState TriggeringInteractable;
        
        [Header("Condition Settings")]
        [SerializeField] private List<OwnerWithSettings> Conditions = new();

        [InspectorButton("GeneratePrerequisites")]
        public bool Generate;

        [HideInInspector] [SerializeField] private InteractionHandler interactionHandler;
        [HideInInspector] [SerializeField] InteractionViewer InteractionViewer;
        
        private void GeneratePrerequisites()
        {
            var prerequisite = new PrerequisiteRecord
            {
                InteractionToExecute = InteractionToExecute,
                TriggerType = TriggerType,
                TriggeringInteractable = TriggeringInteractable
            };

            if (DialogOptionToUnlock != null)
            {
                DialogOptionToUnlock.IsAvailable = false;
                prerequisite.DialogOptionToUnlock = DialogOptionToUnlock;
            }

            if(Conditions != null)
            {   
                prerequisite.Conditions = new List<StateWithSettings>();
                
                foreach (var condition in Conditions)
                {
                    prerequisite.Conditions.Add(new StateWithSettings()
                    {
                        RecordedState = condition.Owner.CurrentState,
                        IsHardCondition = condition.IsHardCondition
                    });
                }
            }
            
            interactionHandler.AddPrerequisite(IsHighPriority, new Trigger(TriggerType, TriggeringInteractable), prerequisite);
            InteractionViewer.AddInteraction(InteractionToExecute.name, IsHighPriority, new Trigger(TriggerType, TriggeringInteractable), prerequisite);   
            EditorUtility.SetDirty(this);
        }
    }
    
    [Serializable]
    public class OwnerWithSettings
    {
        public WorldStateOwner Owner;
        [Tooltip("If true, the interaction will not execute if this condition is not met (otherwise is will execute the failure reaction)")]
        public bool IsHardCondition;
    }
}