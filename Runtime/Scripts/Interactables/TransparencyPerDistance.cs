using Runtime.Scripts.Core;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class TransparencyPerDistance : MonoBehaviour
    {
        // [SerializeField] private SphereCollider collider;
        [SerializeField] private TriggerArea triggerArea;
        private float colliderRadius = 1f;
        [SerializeField] private PlayerController player;
        [SerializeField] private bool playerIsNear;
        [SerializeField] private Renderer doorRenderer;

        private void OnEnable()
        {
            doorRenderer = GetComponent<Renderer>();
            
            triggerArea = gameObject.GetComponentInChildren<TriggerArea>();
            colliderRadius = triggerArea.GetComponent<SphereCollider>().radius;
            triggerArea.OnPlayerEntered += HandlePlayerEntered;
            triggerArea.OnPlayerExited += HandlePlayerExited;
        }

        private void HandlePlayerExited()
        {
            playerIsNear = false;
            player = null;
        }

        private void HandlePlayerEntered(PlayerController player)
        {
            playerIsNear = true;
            this.player = player;
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     player = other.GetComponent<PlayerController>();
        //
        //     if(player != null)
        //     {
        //         playerIsNear = true;
        //     }
        // }

        // private void OnTriggerExit(Collider other)
        // {
        //     if (other.GetComponent<PlayerController>() == null) return;
        //
        //     playerIsNear = false;
        //     player = null;
        // }

        private void Update()
        {
            if (!playerIsNear) return;

            SetTransparency();
        }

        private void SetTransparency()
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            float transparency = distance / (colliderRadius * 12f);

            var color = doorRenderer.material.color;
            color.a = transparency;
            doorRenderer.material.color = color;
        }
    }
}