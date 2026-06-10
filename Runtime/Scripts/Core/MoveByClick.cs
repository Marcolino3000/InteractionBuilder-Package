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
        [SerializeField] private float interactionStoppingDistance;
        [SerializeField] private float standardStoppingDistance;

        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private NavMeshAgent navMeshAgent;

        private bool isMoving;
        private MoveDirection lastMoveDirection;
        
        private void Update()
        {
            if (useNavMeshMovement && navMeshAgent != null)
            {
                CheckNavMeshMovementState();
            }
        }

        private void CheckNavMeshMovementState()
        {
            // Check if the agent is calculating a path or hasn't reached its destination yet
            bool isCurrentlyMoving = navMeshAgent.pathPending || 
                                     navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;

            if (!isCurrentlyMoving && isMoving)
            {
                isMoving = false;
                OnNavMeshMovementEnded?.Invoke();
            }
        }
        
        public void HandleMouseClick(bool isInteraction, Vector3 position)
        {
            navMeshAgent.stoppingDistance = isInteraction ? interactionStoppingDistance : standardStoppingDistance;

            if (isInteraction)
            {
                MoveByNavMesh(position);
                return;
            }
            
            if(useNavMeshMovement)
                MoveByNavMesh(position);
            else
                MoveUsingPlayerController(position);
        }

        private void MoveByNavMesh(Vector3 position)
        {
            SetAgentDestination(position);
        }

        private void MoveUsingPlayerController(Vector3 position)
        {
            playerController.MoveToTargetPosition(new Vector2(position.x, position.z + verticalPositionOffset));
        }

        public void SetAgentDestination(Vector3 position)
        {
            navMeshAgent.SetDestination(new Vector3(position.x, position.y, position.z));
            
            float deltaX = position.x - transform.position.x;
            MoveDirection currentDirection = deltaX > 0 ? MoveDirection.Right : MoveDirection.Left;

            if (!isMoving || currentDirection != lastMoveDirection)
            {
                isMoving = true;
                lastMoveDirection = currentDirection;
                OnNavMeshMovementStarted?.Invoke(lastMoveDirection);
            }
        }
    }
}