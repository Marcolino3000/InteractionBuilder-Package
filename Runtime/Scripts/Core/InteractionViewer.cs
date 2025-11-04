using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "Managers/InteractionViewer")]
    public class InteractionViewer : ScriptableObject
    {
        [SerializeField] private InteractionHandler interactionHandler;
        public List<TriggerToPrereqs> triggerToPrerequisites;
        // [SerializeField] private List<Interaction> interactions;

        public void AddInteraction(string interactionName, bool highPriority, Trigger trigger, PrerequisiteRecord record)
        {
            // interactions.Add(new Interaction(interactionName, highPriority, trigger, record));
            
            AddToList(trigger, record, interactionName, highPriority);
        }

        private void AddToList(Trigger newTrigger, PrerequisiteRecord record, string interactionName, bool highPriority)
        {
            var trigger = triggerToPrerequisites.Find(t => t.Trigger.Equals(newTrigger));

            if (trigger != null)
            {
                trigger.Prerequisites.Add(new PrereqNamePriority(record, interactionName, highPriority));
            }

            else
            {
                triggerToPrerequisites.Add(new TriggerToPrereqs(newTrigger, new List<PrereqNamePriority> { new (record, interactionName, highPriority) }));
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += mode =>
            {
                if (mode == PlayModeStateChange.EnteredPlayMode)
                {
                    SetupInteractionHandler();
                } 
            };
        }

        private void SetupInteractionHandler()
        {
            foreach (var triggerToPrerequisite in triggerToPrerequisites)
            {
                foreach (var prerequisite in triggerToPrerequisite.Prerequisites)
                {
                    interactionHandler.AddPrerequisite(prerequisite.HighPriority, triggerToPrerequisite.Trigger, prerequisite.Record);        
                }
            }
            
        }
    }


    [Serializable]
    public class TriggerToPrereqs
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