using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Scripts.PlayerInput
{
    public class PlayerController : MonoBehaviour
    {
        public event Action OnInteractionTriggered;
    
        [SerializeField] private float speed = 30f;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void OnInteract()
        {
            // Debug.Log("Interact");
            OnInteractionTriggered?.Invoke();
        }

        public void OnMove(InputValue value)
        {
            rb.linearVelocity = new Vector3(value.Get<Vector2>().x * speed, 0, value.Get<Vector2>().y * speed);
        }
    }
}
