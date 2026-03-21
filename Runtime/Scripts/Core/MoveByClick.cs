using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class MoveByClick : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float verticalPositionOffset;
        
        private Camera cam;
        private Vector3 targetPosition;
        private bool isMoving = false;

        private void Start()
        {
            cam = Camera.main;
        }

        public void HandleMouseClick()
        {
            if (playerController == null || cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                targetPosition = hit.point;
                playerController.MoveToTargetPosition(new Vector2(targetPosition.x, targetPosition.z + verticalPositionOffset));
            }
        }
    }
}