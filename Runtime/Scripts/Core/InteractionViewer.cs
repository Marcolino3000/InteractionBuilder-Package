using System;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "Managers/InteractionViewer")]
    public class InteractionViewer : ScriptableObject, ISceneSetupCallbackReceiver
    {
        [SerializeField] private InteractionHandler interactionHandler;
        public List<TriggerToPrereqs> triggerToPrerequisites;

        private void OnValidate()
        {
            SetupInteractionHandler();
        }
 
        [ContextMenu("Reset Interactions")]
        private void ResetInteractions()
        {
            triggerToPrerequisites = new List<TriggerToPrereqs>();
        }
        
        [ContextMenu("Write Changes to Handler")]
        private void SetupInteractionHandler()
        {
            interactionHandler.triggersToPrerequisitesLowPrio = null;
            interactionHandler.triggersToPrerequisitesHighPrio = null;
            
            foreach (var triggerToPrerequisite in triggerToPrerequisites)
            {
                foreach (var prerequisite in triggerToPrerequisite.Prerequisites)
                {
                    interactionHandler.AddPrerequisite(prerequisite.HighPriority, triggerToPrerequisite.Trigger, prerequisite.Record);        
                }
            }
            
            interactionHandler.Setup();
        }
        
        public void AddInteraction(string interactionName, bool highPriority, Trigger trigger, PrerequisiteRecord record)
        {
            // interactions.Add(new Interaction(interactionName, highPriority, trigger, record));
            
            AddToList(trigger, record, interactionName, highPriority);
            
            DeleteTriggersWithNoPrerequisites();
            SortByInteractableAndTriggerType();
        }

        public void DeleteTriggersWithNoPrerequisites()
        {
            triggerToPrerequisites.RemoveAll(t => t.Prerequisites.Count == 0 || t.Prerequisites == null);
        }

        private void AddToList(Trigger newTrigger, PrerequisiteRecord record, string interactionName, bool highPriority)
        {
            var trigger = triggerToPrerequisites.Find(t => t.Trigger == newTrigger);

            if (trigger != null)
            {
                trigger.Prerequisites.Add(new PrereqNamePriority(record, interactionName, highPriority));
            }

            else
            {
                triggerToPrerequisites.Add(new TriggerToPrereqs(newTrigger, new List<PrereqNamePriority> { new (record, interactionName, highPriority) }));
            }
        }

        public void OnSceneSetup()
        {
            SetupInteractionHandler();
        }

        public void DeletePrereq(PrerequisiteRecord prerequisite)
        {
            foreach (var triggerToPrerequisite in triggerToPrerequisites)
            {
                // PrerequisiteRecord toRemove;

                foreach (var prereq in triggerToPrerequisite.Prerequisites)
                {
                    if(prereq.Record == prerequisite)
                    {
                        var toRemove = prereq;
                        triggerToPrerequisite.Prerequisites.Remove(toRemove);
                        return;                     
                    }
                }
                // if (toRemove != null)
                    
            }
            
            // foreach (var triggerToPrerequisite in triggerToPrerequisites)
            // {
            //     triggerToPrerequisite.Prerequisites.RemoveAll(p => p.Record == prerequisite);
            // }
        }

        public void SortByInteractableAndTriggerType()
        {
            triggerToPrerequisites.Sort((a, b) =>
            {
                bool aValid = a?.Trigger?.TriggeringInteractable?.Interactable != null;
                bool bValid = b?.Trigger?.TriggeringInteractable?.Interactable != null;

                if (!aValid && !bValid) return 0;
                if (!aValid) return 1;
                if (!bValid) return -1;
                
                string nameA = a?.Trigger?.TriggeringInteractable?.Interactable?.name ?? string.Empty;
                string nameB = b?.Trigger?.TriggeringInteractable?.Interactable?.name ?? string.Empty;
                
                int nameCompare = string.Compare(nameA, nameB, StringComparison.Ordinal);
                if (nameCompare != 0)
                    return nameCompare;
                
                // Compare TriggerType
                return a.Trigger.TriggerType.CompareTo(b.Trigger.TriggerType);
            });
        }
    }


    [Serializable]
    public record TriggerToPrereqs
    {
        public TriggerToPrereqs(Trigger trigger, List<PrereqNamePriority> record)
        {
            Trigger = trigger;
            Prerequisites = record;
        }
        
        public Trigger Trigger;
        public List<PrereqNamePriority> Prerequisites;
    }

    [Serializable]
    public class PrereqNamePriority
    {
        public PrereqNamePriority(PrerequisiteRecord record, string name,  bool highPriority)
        {
            Record = record;
            Name = name;
            HighPriority = highPriority;
        }
        public PrerequisiteRecord Record;
        public string Name;
        public bool HighPriority;
    }
    
    [Serializable]
    public class Interaction
    {
        public string Name;
        public bool HighPriority;
        public Trigger Trigger;
        public PrerequisiteRecord Prerequisites;

        public Interaction(string interactionName, bool highPriority, Trigger trigger, PrerequisiteRecord record)
        {
            Name = interactionName;
            HighPriority = highPriority;
            Trigger = trigger;
            Prerequisites = record;
        }
    }
}