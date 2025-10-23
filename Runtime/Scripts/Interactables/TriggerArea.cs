using System;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class TriggerArea : MonoBehaviour
    {
        public event Action<PlayerController> OnPlayerEntered;
        public event Action<Sauerteig> OnSauerteigEntered;
        public event Action OnPlayerExited;
        private void OnTriggerEnter(Collider other)
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            var sauerteig = other.gameObject.GetComponent<Sauerteig>();

            if(player != null)
            {
                OnPlayerEntered?.Invoke(player);
            }

            if (sauerteig != null)
            {
                OnSauerteigEntered?.Invoke(sauerteig);
                Debug.Log("sauerteig entered");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Debug.Log(other.name + " exited");
            OnPlayerExited?.Invoke();
        }
    }
}
