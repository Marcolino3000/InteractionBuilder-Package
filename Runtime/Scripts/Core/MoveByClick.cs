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

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (isMoving)
            {
                Vector3 direction = (targetPosition - playerController.transform.position);
                direction.y = 0;
                
                if (direction.magnitude < 0.1f)
                {
                    playerController.OnMove(Vector2.zero);
                    isMoving = false;
                }
                else
                {
                    Vector2 moveInput = new Vector2(direction.x, direction.z).normalized;
                    playerController.OnMove(moveInput);
                }
            }
        }

        public void HandleMouseClick()
        {
            if (playerController == null || cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                targetPosition = hit.point;
                targetPosition.z += verticalPositionOffset;
                isMoving = true;
            }
        }
    }
}