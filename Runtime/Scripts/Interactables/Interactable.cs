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
        [SerializeField] private InteractableState data;

        [SerializeField] private SphereCollider _collider;
        
        private TriggerArea _triggerArea;
        private PlayerController _player;


        private void OnEnable()
        {
            _triggerArea = GetComponentInChildren<TriggerArea>();
            _collider = GetComponentInChildren<SphereCollider>();
            
            _triggerArea.OnTriggerEntered += player =>
            {
                _player = player;
                
                OnEnteredTriggerArea?.Invoke(InteractionTriggerVia.EnteringTriggerArea, data);
                
                SubscribeToPlayerInteraction(player);
            };  
            
            _triggerArea.OnTriggerExited += () =>
            {
                UnsubscribeFromPlayerInteraction();

                OnExitedTriggerArea?.Invoke(InteractionTriggerVia.ExitingTriggerArea, data);
            };

            data.OnInteractionFeedback += HandleInteractionFeedback;
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
            OnInteractionStarted?.Invoke(InteractionTriggerVia.ButtonPress, data);
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