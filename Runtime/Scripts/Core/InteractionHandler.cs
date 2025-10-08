using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Scripts.Interactables;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionHandler")]
    public class InteractionHandler : SerializedScriptableObject
    {
        [SerializeField] Dictionary<Trigger, List<PrerequisiteRecord>> triggersToPrerequisitesHighPrio;
        [SerializeField] Dictionary<Trigger, List<PrerequisiteRecord>> triggersToPrerequisitesLowPrio;

        [HideInInspector] [SerializeField] private PrerequisitesGenerator PrerequisitesGenerator;

        private void HandleInteractionTrigger(InteractionTriggerVia triggerType,
            InteractableState triggeringInteractable)
        {
            var trigger = new Trigger(triggerType, triggeringInteractable);

            if (triggersToPrerequisitesHighPrio.TryGetValue(trigger, out List<PrerequisiteRecord> prereqsHighPrio))
            {
                foreach (var prereqHighPrio in prereqsHighPrio)
                {
                    prereqHighPrio.TryExecuteInteraction();
                }

                return;
            }

            if (triggersToPrerequisitesLowPrio.TryGetValue(trigger, out List<PrerequisiteRecord> prereqsLowPrio))
            {
                foreach (var prereqLowPrio in prereqsLowPrio)
                {
                    prereqLowPrio.TryExecuteInteraction();
                }
            }
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
                interactable.OnEnteredTriggerArea += HandleInteractionTrigger;
                interactable.OnInteractionStarted += HandleInteractionTrigger;
                interactable.OnExitedTriggerArea += HandleInteractionTrigger;
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

        private void Setup()
        {
            FindClients();
            FindEventSystem();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode)
                {
                    Setup();
                }
            };
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
        ButtonPress
    }

    [Serializable]
    public record Trigger()
    {
        public Trigger(InteractionTriggerVia triggerType, InteractableState triggeringInteractable) : this()
        {
            TriggerType = triggerType;
            TriggeringInteractable = triggeringInteractable;
        }

        public InteractionTriggerVia TriggerType;
        public InteractableState TriggeringInteractable;
    }
}