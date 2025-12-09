using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "InteractionBuilder/ScriptedSequence")]
    public class ScriptedSequence : ScriptableObject
    {
        public List<Vector3> Waypoints;
        public PlayerController PlayerController;
        public BoxCollider PlayerCollider;
        public event Action OnSequenceCompleted;
        private MonoBehaviour coroutineHost;
        private int currentWaypointIndex = 0;
        private float waypointThreshold = 0.2f;
        private Coroutine moveCoroutine;
        public float moveSpeed = 1f;

        public void StartSequence(MonoBehaviour host, PlayerController playerController, BoxCollider playerCollider)
        {
            Debug.Log("Scripted Sequence Started");
            coroutineHost = host;
            PlayerController = playerController;
            PlayerCollider = playerCollider;
            PlayerCollider.enabled = false;
            currentWaypointIndex = 0;
            if (moveCoroutine != null)
                coroutineHost.StopCoroutine(moveCoroutine);
            moveCoroutine = coroutineHost.StartCoroutine(MoveToWaypoints());
        }

        private IEnumerator MoveToWaypoints()
        {
            while (currentWaypointIndex < Waypoints.Count)
            {
                var target = Waypoints[currentWaypointIndex];
                while (Vector3.Distance(PlayerController.transform.position, target) > waypointThreshold)
                {
                    Vector3 direction = (target - PlayerController.transform.position);
                    Vector2 moveInput = new Vector2(direction.x, direction.z).normalized * moveSpeed;
                    Debug.Log($"MoveInput: {moveInput}, Position: {PlayerController.transform.position}");
                    PlayerController.OnMove(moveInput);
                    yield return null;
                }
                currentWaypointIndex++;
            }

            PlayerCollider.enabled = true;
            Debug.Log("movement finished");
            OnSequenceCompleted?.Invoke();
        }
    }
}