using System;
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
        
        [Header("Toggle Settings")]
        [SerializeField] private Sprite StatusOnSprite;
        [SerializeField] private Sprite StatusOffSprite;
        
        public override StateData CurrentState => new ToggleableStateDate { Owner = this, ToggleState = ToggleState };
    
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
            
            GetSpriteRenderer().sprite = ToggleState ? StatusOnSprite : StatusOffSprite;
        }

        private SpriteRenderer GetSpriteRenderer()
        {
            var renderer = Interactable.GetComponent<SpriteRenderer>();
            if(renderer != null)
                return renderer;

            renderer = Interactable.GetComponentInParent<SpriteRenderer>();
            if(renderer != null)
                return renderer;
            
            Debug.LogWarning($"No SpriteRenderer found on {Interactable.name} or its parents. Cannot update sprite.");
            
            return null;
        }

        [Serializable]
        private record ToggleableStateDate : StateData
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