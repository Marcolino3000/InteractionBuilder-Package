using Runtime.Scripts.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Scripts.PlayerInput
{
    public class InputDispatcher : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Sauerteig sauerteig;
        // [SerializeField] private MapHandler mapHandler;
        
        private void OnMove(InputValue value)
        {
            playerController.OnMove(value);
        }

        private void OnInteract()
        {
            playerController.OnInteract();
        }
        
        private void OnToggleMap()
        {
            // mapHandler.OnToggleMap();
        }
        
        private void OnActivateRadar()
        {
            sauerteig.ActivateRadar();
        }
    }
}