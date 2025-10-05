using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "ScriptableObjects/InteractionPrerequisites")]
    public class InteractionPrerequisites : ScriptableObject
    {
        public InteractableState InteractableData;
        public string InteractableState;
        public InteractionData InteractionData;
        public string InteractionState;
    }
}