using System;
using Runtime.Scripts.Core;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Interactable : MonoBehaviour
    {
        public event Action<InteractionTriggerVia, InteractableState> OnEnteredTriggerArea;
        public event Action<InteractionTriggerVia, InteractableState> OnInteractionStarted;
        public event Action<InteractionTriggerVia, InteractableState> OnExitedTriggerArea;
        public event Action OnInteractionSuccessful;

        [SerializeField] private float triggerAreaRadius = 0.5f;
        public InteractableState Data;
        
        [SerializeField] private SphereCollider _collider;
        
        private InteractableDisplay _interactableDisplay;
        private TriggerArea _triggerArea;
        private PlayerController _player;
        private Action<InteractableState> _interactableDiscoveredCallback;


        private void OnEnable()
        {
            _triggerArea = GetComponentInChildren<TriggerArea>();
            _collider = GetComponentInChildren<SphereCollider>();
            _interactableDisplay = GetComponentInChildren<InteractableDisplay>();
            
            _triggerArea.OnPlayerEntered += player =>
            {
                _player = player;
                
                OnEnteredTriggerArea?.Invoke(InteractionTriggerVia.EnteringTriggerArea, Data);
                
                SubscribeToPlayerInteraction(player);
            };

            _triggerArea.OnSauerteigEntered += sauerteig =>
            {
                if(Data.AwarenessLevel == sauerteig.awarenessLevel)
                {
                    _interactableDiscoveredCallback = sauerteig.HandleInteractableDiscovered;
                    _interactableDisplay.Show();
                }
            };
            
            _triggerArea.OnPlayerExited += () =>
            {
                UnsubscribeFromPlayerInteraction();

                OnExitedTriggerArea?.Invoke(InteractionTriggerVia.ExitingTriggerArea, Data);
            };

            Data.OnInteractionFeedback += HandleInteractionFeedback;
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
            _player.OnInteractionTriggered -= HandlePlayerTriggeredInteraction;
            _player = null;
        }

        private void HandlePlayerTriggeredInteraction()
        {
            OnInteractionStarted?.Invoke(InteractionTriggerVia.ButtonPress, Data);

            _interactableDiscoveredCallback?.Invoke(Data);
            
            if(_interactableDiscoveredCallback != null)
                _interactableDisplay.MarkAsFound();
            
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
    }
}