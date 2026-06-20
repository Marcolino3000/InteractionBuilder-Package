using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.Scripts.Core
{
    public class SequenceRunner : MonoBehaviour
    {
        public static event Action<bool> OnSequenceRunningChanged; 
        
        [Header("Settings")]
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float waypointThreshold = 0.2f;
        [SerializeField] private bool disableColliderDuringSequence;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private bool useProximityCheck;
        
        [Header("Debug")]
        [SerializeField] private bool isSequenceRunning;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private List<Waypoint> Waypoints;
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private MoveByClick moveByClick;

        private Coroutine moveCoroutine;
        private List<Reaction> allReactions;
        private Action ReactionFinishedCallback;
        private bool waypointReached;

        private void OnEnable()
        {
            allReactions = new List<Reaction>(Resources.LoadAll<Reaction>(""));

            foreach (var reaction in allReactions)
            {
                reaction.OnStartSequence += StartMovingPlayer;
            }

            if (moveByClick != null)
                moveByClick.OnNavMeshMovementEnded += HandleNavMeshMovementEnded;
        }

        private void OnDisable()
        {
            if (allReactions != null)
            {
                foreach (var reaction in allReactions)
                    reaction.OnStartSequence -= StartMovingPlayer;
            }

            if (moveByClick != null)
                moveByClick.OnNavMeshMovementEnded -= HandleNavMeshMovementEnded;
        }

        private void HandleNavMeshMovementEnded()
        {
            waypointReached = true;
        }
        
        [ContextMenu("start sequence")]
        public void StartMovingPlayer(List<Waypoint> waypoints, Action action)
        {
            Waypoints = waypoints;
            ReactionFinishedCallback = action;
            
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            
            if(useProximityCheck)
                moveCoroutine = StartCoroutine(MoveToWaypoints());
            else
                moveCoroutine = StartCoroutine(MoveToWaypointsNavMesh());
        }

        private IEnumerator MoveToWaypointsNavMesh()
        {
            isSequenceRunning = true;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);

            if (boxCollider != null && disableColliderDuringSequence)
                boxCollider.enabled = false;

            if (Waypoints == null || Waypoints == null || Waypoints.Count == 0)
            {
                Debug.LogWarning("No waypoints set.");
                yield break;
            }

            int currentWaypointIndex = 0;

            while (currentWaypointIndex < Waypoints.Count)
            {
                var waypoint = Waypoints[currentWaypointIndex];
                var target = waypoint.Position;

                if(waypoint.ReactionAtStart != null)
                    waypoint.ReactionAtStart.Execute ();

                waypointReached = false;
                // moveByClick.SetAgentDestination(target);
                moveByClick.HandleMouseClick(false, target);

                // MoveByClick raises OnNavMeshMovementEnded once the agent has stopped at the
                // destination; that is our "waypoint reached" signal.
                yield return new WaitUntil(() => waypointReached || !isSequenceRunning);

                if(waypoint.ReactionAtStop != null)
                    waypoint.ReactionAtStop.Execute();

                if (waypoint.WaitTime > 0f)
                {
                    yield return new WaitForSeconds(waypoint.WaitTime);
                }

                currentWaypointIndex++;
            }

            if (boxCollider != null && disableColliderDuringSequence)
                boxCollider.enabled = true;

            ReactionFinishedCallback?.Invoke();
            isSequenceRunning = false;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);
        }

        private IEnumerator MoveToWaypoints()
        {
            isSequenceRunning = true;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);
            
            if (boxCollider != null && disableColliderDuringSequence)
                boxCollider.enabled = false;
            
            if (Waypoints == null || Waypoints == null || Waypoints.Count == 0)
            {
                Debug.LogWarning("No waypoints set.");
                yield break;
            }
            
            int currentWaypointIndex = 0;
            
            while (currentWaypointIndex < Waypoints.Count)
            {
                var waypoint = Waypoints[currentWaypointIndex];
                var target = waypoint.Position;

                if(waypoint.ReactionAtStart != null)
                    waypoint.ReactionAtStart.Execute ();

                yield return playerController.MoveToTargetPositionCoroutine(new Vector2(target.x, target.z));

                if(waypoint.ReactionAtStop != null)
                    waypoint.ReactionAtStop.Execute();

                if (waypoint.WaitTime > 0f)
                {
                    yield return new WaitForSeconds(waypoint.WaitTime);
                }

                currentWaypointIndex++;
            }

            if (boxCollider != null && disableColliderDuringSequence)
                boxCollider.enabled = true;

            ReactionFinishedCallback?.Invoke();
            isSequenceRunning = false;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);
        }
    }
}