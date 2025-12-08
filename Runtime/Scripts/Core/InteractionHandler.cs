using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Nodes.Decorator;
using Runtime.Scripts.Interactables;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionHandler")]
    public class InteractionHandler : SerializedScriptableObject
    {
        [SerializeField] public Dictionary<Trigger, List<PrerequisiteRecord>> triggersToPrerequisitesHighPrio;
        [SerializeField] public Dictionary<Trigger, List<PrerequisiteRecord>> triggersToPrerequisitesLowPrio;

        [HideInInspector] [SerializeField] private PrerequisitesGenerator PrerequisitesGenerator;
        
        [SerializeField] private List<InteractionData> currentInteractions;

        private void HandleTriggerViaInteractable(InteractionTriggerVia triggerType,
            InteractableState triggeringInteractable)
        {
            var trigger = new Trigger(triggerType, triggeringInteractable);

            FindInteractions(trigger);
        }

        private void HandleTriggerViaDialog(DialogOptionNode option)
        {
            var trigger = new Trigger(option);
            
            FindInteractions(trigger);
        }

        private void FindInteractions(Trigger trigger)
        {
            var newInteractions = new List<InteractionData>();
            
            if (triggersToPrerequisitesHighPrio.TryGetValue(trigger, out List<PrerequisiteRecord> prereqsHighPrio))
            {
                foreach (var prereqHighPrio in prereqsHighPrio)
                {
                    prereqHighPrio.Execute();
                }

                return;
            }

            if (triggersToPrerequisitesLowPrio.TryGetValue(trigger, out List<PrerequisiteRecord> prereqsLowPrio))
            {
                foreach (var prereqLowPrio in prereqsLowPrio)
                {
                    var interaction = prereqLowPrio.Execute();
                    
                    if(interaction != null)
                        newInteractions.Add(interaction);
                }
            }
            
            // ResetCurrentInteractions(newInteractions);
        }

        private void ResetCurrentInteractions(List<InteractionData> newInteractions)
        {
            if(newInteractions.Count  == 0)
                return;
            
            foreach (var currentInteraction in currentInteractions)
            {
                // currentInteraction.HandleInteractionStop();
            }
            
            currentInteractions = newInteractions;
        }

        private void FindClients()
        {
            var interactables = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<Interactable>().ToList();

            if (interactables.Count == 0)
            {
                Debug.Log("No objects found implementing a interactable interface.");
            }

            foreach (var interactable in interactables)
            {
                interactable.OnEnteredTriggerArea += HandleTriggerViaInteractable;
                interactable.OnInteractionStarted += HandleTriggerViaInteractable;
                interactable.OnExitedTriggerArea += HandleTriggerViaInteractable;
            }
        }

        private void FindEventSystem()
        {
            if (Application.isPlaying)
            {
                if (EventSystem.current != null)
                    return;

                var eventSystem = new GameObject("[TEMP] EventSystem - added by InteractionHandler");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Debug.LogWarning("Added EventSystem because none was found in the scene.");
            }
        }

        public void Setup()
        {
            FindClients();
            FindEventSystem();
            SetDialogOptionsAvailability();
            ResetLastInteractions();
            SubscribeToDialogEvents();
        }

        private void SubscribeToDialogEvents()
        {
            // DialogTreeRunner.OnDialogRunningStatusChanged += HandleDialogStatusChanged;
            DialogTreeRunner.DialogNodeSelected += HandleTriggerViaDialog;
        }

        private void HandleDialogStatusChanged(bool isRunning, DialogTree dialogTree)
        {
            currentInteractions
                .Where(i => 
                    (i?.successReaction?.DialogTree == dialogTree) ||
                    (i?.failureReaction?.DialogTree == dialogTree))
                .ForEach(i => 
                {
                    // i.IsRunning = isRunning;
                    // if(!isRunning)
                        // i.HandleInteractionStop();
                });
        }

        private void ResetLastInteractions()
        {
            currentInteractions = new List<InteractionData>();
        }

        private void SetDialogOptionsAvailability()
        {
            foreach (var triggerType in triggersToPrerequisitesLowPrio)
            {
                foreach (var record in triggerType.Value)
                {
                    if(record.DialogOptionToUnlock != null)
                        record.DialogOptionToUnlock.IsAvailable = false;
                }
            }
            
            foreach (var triggerType in triggersToPrerequisitesHighPrio)
            {
                foreach (var record in triggerType.Value)
                {
                    if(record.DialogOptionToUnlock != null)
                        record.DialogOptionToUnlock.IsAvailable = false;
                }
            }
        }
        
        public void AddPrerequisite(bool highPriority, Trigger trigger, PrerequisiteRecord prereq)
        {
            if (triggersToPrerequisitesHighPrio == null)
                triggersToPrerequisitesHighPrio = new Dictionary<Trigger, List<PrerequisiteRecord>>();
            if (triggersToPrerequisitesLowPrio == null)
                triggersToPrerequisitesLowPrio = new Dictionary<Trigger, List<PrerequisiteRecord>>();
            
            AddToDictionary(highPriority ? triggersToPrerequisitesHighPrio : triggersToPrerequisitesLowPrio,
                trigger, prereq);
        }

        private void AddToDictionary(Dictionary<Trigger, List<PrerequisiteRecord>> dictionary, Trigger trigger,
            PrerequisiteRecord prereq)
        {
            if (dictionary.ContainsKey(trigger))
            {
                TryAddToList(dictionary[trigger], prereq);
            }
            else
                dictionary.Add(trigger, new List<PrerequisiteRecord> { prereq });
        }

        private void TryAddToList(List<PrerequisiteRecord> prerequisiteRecords, PrerequisiteRecord prereq)
        {
            if (prerequisiteRecords.Contains(prereq))
                return;
            
            prerequisiteRecords.Add(prereq);
        }
    }

    public enum InteractionTriggerVia
    {
        EnteringTriggerArea,
        ExitingTriggerArea,
        ButtonPress,
        TrustThresholdReached, 
        DialogOptionSelected
    }

    [Serializable]
    public record Trigger()
    {
        public Trigger(InteractionTriggerVia triggerType, InteractableState triggeringInteractable) : this()
        {
            TriggerType = triggerType;
            TriggeringInteractable = triggeringInteractable;
        }

        public Trigger(DialogOptionNode dialogOptionNode) : this()
        {
            TriggeringDialogOption = dialogOptionNode;
            TriggerType = InteractionTriggerVia.DialogOptionSelected;
        }

        public InteractionTriggerVia TriggerType;
        public InteractableState TriggeringInteractable;
        public DialogOptionNode TriggeringDialogOption;
    }
}