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
        public Sprite StatusSpriteOn;
        public Sprite StatusSpriteOff;
        public Vector3 SpriteRotationOn;
        public Vector3 SpriteRotationOff;
        
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
            
            if(StatusSpriteOn != null && StatusSpriteOff != null)
                Interactable.SetSprite(ToggleState ? StatusSpriteOn : StatusSpriteOff);
            
            if(SpriteRotationOn != Vector3.zero || SpriteRotationOff != Vector3.zero)
                Interactable.transform.parent.eulerAngles = ToggleState ? SpriteRotationOn : SpriteRotationOff;
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