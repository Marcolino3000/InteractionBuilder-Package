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
        [SerializeField] private InteractableState InteractableState;
        [SerializeField] private InteractionData InteractionState;
        [Tooltip("If true, the interaction will not execute if this condition is not met (otherwise is will execute the failure reaction)")]
        [SerializeField] public bool IsHardCondition;

        [InspectorButton("GeneratePrerequisites")]
        public bool Generate;

        [HideInInspector] [SerializeField] private InteractionHandler interactionHandler;

        private void GeneratePrerequisites()
        {
            var prereq = CreateRecord(TriggerType, TriggeringInteractable, 
                InteractableState, InteractionState, InteractionToExecute);
            
            interactionHandler.AddPrerequisite(IsHighPriority, new Trigger(TriggerType, TriggeringInteractable), prereq);
            
            EditorUtility.SetDirty(this);
        }
        
        private PrerequisiteRecord CreateRecord(InteractionTriggerVia triggerType, InteractableState triggeringInteractable,
            InteractableState interactableData, InteractionData interactionData, InteractionData interactionToExecute = null)
        {
            var prereq = new PrerequisiteRecord();

            prereq.InteractionToExecute = interactionToExecute;
            prereq.TriggerType = triggerType;
            prereq.TriggeringInteractable = triggeringInteractable;
            
            if(interactableData != null)
            {
                prereq.InteractableData = interactableData;
                prereq.InteractableState = JsonUtility.ToJson(interactableData.State, true);
            }
            
            if(interactionData != null)
            {
                prereq.InteractionData = interactionData;
                prereq.InteractionState = JsonUtility.ToJson(interactionData.State, true);
                prereq.IsHardCondition = IsHardCondition;
            }
            
            return prereq;
        }
    }
}