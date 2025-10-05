using System;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class TriggerArea : MonoBehaviour
    {
        public event Action<PlayerController> OnTriggerEntered;
        public event Action OnTriggerExited;
        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
        
            if (player == null)
            {
                Debug.LogWarning("TriggerArea: Non-player object entered the trigger area. Aborting trigger event.");
                return;
            }
        
            OnTriggerEntered?.Invoke(player);
        }

        private void OnTriggerExit(Collider other)
        {
            // Debug.Log(other.name + " exited");
            OnTriggerExited?.Invoke();
        }
    }
}
