using System;
using Runtime.Scripts.PlayerInput;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Scripts.Core
{
    public class MoveByClick : MonoBehaviour
    {
        public event Action<MoveDirection> OnNavMeshMovementStarted;
        public event Action OnNavMeshMovementEnded;

        [Header("Settings")]
        [SerializeField] private bool useNavMeshMovement;
        [SerializeField] private float verticalPositionOffset;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private LayerMask groundLayer;

        private Camera cam;
        private Vector3 targetPosition;
        private bool isMoving;
        private MoveDirection lastMoveDirection;

        private void Start()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            if (useNavMeshMovement && navMeshAgent != null)
            {
                CheckNavMeshMovementState();
            }
        }

        private void CheckNavMeshMovementState()
        {
            bool wasMoving = isMoving;

            isMoving = !navMeshAgent.isStopped;

            if (isMoving)
            {
                MoveDirection currentDirection = navMeshAgent.velocity.x < 0 ? MoveDirection.Left : MoveDirection.Right;

                if (!wasMoving || currentDirection != lastMoveDirection)
                {
                    OnNavMeshMovementStarted?.Invoke(currentDirection);
                    lastMoveDirection = currentDirection;
                }
            }
            else if (wasMoving && !isMoving)
            {
                OnNavMeshMovementEnded?.Invoke();
            }
        }

        public void HandleMouseClick()
        {
            if (playerController == null || cam == null)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if(useNavMeshMovement)
                MoveByNavMesh(ray);
            else
                MoveUsingPlayerController(ray);
        }

        private void MoveUsingPlayerController(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                targetPosition = hit.point;
                playerController.MoveToTargetPosition(new Vector2(targetPosition.x, targetPosition.z + verticalPositionOffset));
            }
        }

        private void MoveByNavMesh(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                navMeshAgent.SetDestination(hit.point);
            }
        }
    }
}