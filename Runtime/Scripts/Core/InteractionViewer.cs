using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "Managers/InteractionViewer")]
    public class InteractionViewer : ScriptableObject
    {
        // [SerializeField] private List<Interaction> interactions;
        [SerializeReference] public List<TriggerToPrereqs> triggerToPrerequisites;
        
        public void AddInteraction(string interactionName, bool highPriority, Trigger trigger, PrerequisiteRecord record)
        {
            // interactions.Add(new Interaction(interactionName, highPriority, trigger, record));
            
            AddToList(trigger, record);
        }

        private void AddToList(Trigger newTrigger, PrerequisiteRecord record)
        {
            var trigger = triggerToPrerequisites.Find(t => t.Trigger.Equals(newTrigger));

            if (trigger != null)
            {
                trigger.Prerequisites.Add(record);
            }

            else
            {
                triggerToPrerequisites.Add(new TriggerToPrereqs(newTrigger, new List<PrerequisiteRecord> { record }));
            }
        }
    }


    [Serializable]
    public class TriggerToPrereqs
    {
        public TriggerToPrereqs(Trigger trigger, List<PrerequisiteRecord> record)
        {
            Trigger = trigger;
            Prerequisites = record;
        }
        
        public Trigger Trigger;
        public List<PrerequisiteRecord> Prerequisites;
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