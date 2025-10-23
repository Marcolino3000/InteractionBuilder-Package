using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "Managers/InteractionViewer")]
    public class InteractionViewer : ScriptableObject
    {
        [SerializeField] private List<Interaction> interactions;
        
        public void AddInteraction(string interactionName, bool highPriority, Trigger trigger, PrerequisiteRecord record)
        {
            interactions.Add(new Interaction(interactionName, highPriority, trigger, record));
        }
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