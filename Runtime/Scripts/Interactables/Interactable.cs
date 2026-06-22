using System;
using Runtime.Scripts.Core;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Interactable : MonoBehaviour, ISceneSetupCallbackReceiver
    {
        public event Action<InteractionTriggerVia, InteractableState> OnEnteredTriggerArea;
        public Action<InteractionTriggerVia, InteractableState> OnInteractionStarted;
        public event Action<InteractionTriggerVia, InteractableState> OnExitedTriggerArea;
        public event Action OnInteractionSuccessful;

        [SerializeField] private float triggerAreaRadius = 0.5f;
        public InteractableState Data;

        public bool Spawned;
        public bool Found;
        public bool MarkAsFoundWhenClicked;
        
        [SerializeField] private SphereCollider _collider;
        
        private InteractableDisplay _interactableDisplay;
        private TriggerArea _triggerArea;
        private PlayerController _player;
        private Action<InteractableState> _interactableDiscoveredCallback;


        private void OnEnable()
        {
            if(Data != null)
                Data.SetInteractable(this);
            
            _triggerArea = GetComponentInChildren<TriggerArea>();
            _collider = GetComponentInChildren<SphereCollider>();
            _interactableDisplay = GetComponentInChildren<InteractableDisplay>();
            
            _triggerArea.OnPlayerEntered += player =>
            {
                _player = player;
                
                OnEnteredTriggerArea?.Invoke(InteractionTriggerVia.EnterTrigger, Data);
                
                SubscribeToPlayerInteraction(player);
            };

            _triggerArea.OnSauerteigEntered += sauerteig =>
            {
                if (Data.AwarenessLevel == AwarenessLevel.NotSet)
                    return;
                
                if(Data.AwarenessLevel <= sauerteig.State.CurrentLevel)
                {
                    // _interactableDiscoveredCallback = sauerteig.HandleInteractableDiscovered;
                    // _interactableDisplay.ShowPulsatingSpecialOutline();
                }
            };
            
            _triggerArea.OnPlayerExited += () =>
            {
                UnsubscribeFromPlayerInteraction();
                // _interactableDisplay.HideSpecialOutline();
                
                OnExitedTriggerArea?.Invoke(InteractionTriggerVia.ExitTrigger, Data);
            };

            Data.OnInteractionFeedback += HandleInteractionFeedback;
            
            if(Data is Toggleable toggleable)
            {
                if(toggleable.StatusSpriteOn != null && toggleable.StatusSpriteOff != null)
                    SetSprite(toggleable.ToggleState ? toggleable.StatusSpriteOn : toggleable.StatusSpriteOff);
                
                transform.parent.eulerAngles =
                    toggleable.ToggleState ? toggleable.SpriteRotationOn : toggleable.SpriteRotationOff;
            }
        }
        
        public void SetSprite(Sprite sprite)
        {
            GetSpriteRenderer().sprite = sprite;
        }
        
        private SpriteRenderer GetSpriteRenderer()
        {
            var renderer = GetComponent<SpriteRenderer>();
            
            if(renderer != null)
                return renderer;

            renderer = GetComponentInParent<SpriteRenderer>();
            if(renderer != null)
                return renderer;
            
            Debug.LogError($"No SpriteRenderer found on {name} or its parents. Cannot update sprite.");
            
            return null;
        }

        private void SubscribeToPlayerInteraction(PlayerController player)
        {
            if(player == null)
            {
                Debug.LogWarning("Interactable: Player reference is null upon entering trigger area. Aborting interaction setup.");
                return;
            }

            _player.OnInteractionTriggered += HandlePlayerTriggeredInteraction;
        }

        private void UnsubscribeFromPlayerInteraction()
        {
            if(_player == null)
                return;
            
            _player.OnInteractionTriggered -= HandlePlayerTriggeredInteraction;
            _player = null;
        }

        private void HandlePlayerTriggeredInteraction()
        {
            OnInteractionStarted?.Invoke(InteractionTriggerVia.ButtonPress, Data);

            _interactableDiscoveredCallback?.Invoke(Data);
            
            // if(_interactableDiscoveredCallback != null)
            //     _interactableDisplay.MarkAsFound();
            
            _interactableDiscoveredCallback = null;
        }

        private void HandleInteractionFeedback()
        {
            OnInteractionSuccessful?.Invoke();
        }

        private void OnValidate()
        {
            if (_collider != null)
            {
                _collider.radius = triggerAreaRadius;
            }
        }

        public void OnSceneSetup()
        {
            Found = false;
        }
    }
}