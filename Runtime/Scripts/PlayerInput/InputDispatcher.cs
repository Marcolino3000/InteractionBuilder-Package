using Runtime.Scripts.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Scripts.PlayerInput
{
    public class InputDispatcher : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private Raycaster raycaster;
        // [SerializeField] private MapHandler mapHandler;
        
        // private InputAction leftMouseClick;
        //
        // private void Awake() 
        // {
        //     leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
        //     leftMouseClick.performed += ctx => LeftMouseClicked();
        //     leftMouseClick.Enable();
        // }
        //
        // private void LeftMouseClicked() 
        // {
        //     raycaster.HandleMouseClick();
        // }
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

        private void OnClickObject()
        {
            // Debug.Log("Click");
            // raycaster.HandleMouseClick();
        }
    }
}