using System;
using System.Collections.Generic;
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

        [Header("Prerequisites for interaction")]
        [Header("Trigger Settings")]
        [SerializeField] private bool IsHighPriority;
        [SerializeField] private InteractionTriggerVia TriggerType;
        [SerializeField] private InteractableState TriggeringInteractable;
        
        [Header("Condition Settings")]
        // [SerializeField] private InteractableState InteractableState;
        // [SerializeField] private InteractionData InteractionState;
        [Tooltip("If true, the interaction will not execute if this condition is not met (otherwise is will execute the failure reaction)")]
        // [SerializeField] public bool IsHardCondition;
        
        [SerializeField] private List<OwnerWithSettings> Conditions = new();

        [InspectorButton("GeneratePrerequisites")]
        public bool Generate;

        [HideInInspector] [SerializeField] private InteractionHandler interactionHandler;

        private void GeneratePrerequisites()
        {
            var prerequisite = new PrerequisiteRecord
            {
                InteractionToExecute = InteractionToExecute,
                TriggerType = TriggerType,
                TriggeringInteractable = TriggeringInteractable
            };

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
            
            EditorUtility.SetDirty(this);
        }
        
        // private PrerequisiteRecord CreateRecord(InteractionTriggerVia triggerType, InteractableState triggeringInteractable,
        //     InteractionData interactionToExecute)
        // {
        //     
        //     
        //     // if(interactionData != null)
        //     // {
        //     //     prereq.InteractionState = JsonUtility.ToJson(interactionData.State, true);
        //     //     prereq.IsHardCondition = IsHardCondition;
        //     // }
        //     
        //     return prereq;
        // }
    }
    
    [Serializable]
    public class OwnerWithSettings
    {
        public WorldStateOwner Owner;
        public bool IsHardCondition;
    }
}