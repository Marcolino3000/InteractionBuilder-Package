using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Interactable/Toggleable")]
    public class Toggleable : InteractableState
    {
        [Header("Toggleable Status")]
        public bool ToggleState;
        public string StatusDescription;

        [Header("Status Descriptions")] 
        public string StatusOn;
        public string StatusOff;
        
        public override StateData State => new ToggleableStateDate { Owner = this, Name = name, ToggleState = ToggleState };
    
        public override void HandleInteraction()
        {
            base.HandleInteraction();
            
            Toggle();
        }

        public void OnValidate()
        {
            StatusDescription = ToggleState ? StatusOn : StatusOff;
        }

        private void Toggle()
        {
            ToggleState = !ToggleState;
            StatusDescription = ToggleState ? StatusOn : StatusOff;
        }

        private class ToggleableStateDate : StateData
        {
            public bool ToggleState;
        }

        public override void SetState(StateData state)
        {
            if (state is ToggleableStateDate stateData)
            {
                ToggleState = stateData.ToggleState;
                StatusDescription = ToggleState ? StatusOn : StatusOff;
            }
        }
        
    }
}