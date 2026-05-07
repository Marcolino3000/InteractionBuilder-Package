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

        private void OnEnable()
        {
            allReactions = new List<Reaction>(Resources.LoadAll<Reaction>(""));

            foreach (var reaction in allReactions)
            {
                reaction.OnStartSequence += StartMovingPlayer;
            }
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

                moveByClick.SetAgentDestination(target);
                
                yield return CheckIfWaypointReached(waypoint);

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

        private IEnumerator CheckIfWaypointReached(Waypoint waypoint)
        {
            if (waypoint == null)
                yield break;
            
            var target = waypoint.Position;
            var target2 = new Vector2(target.x, target.z);

            // If we have a NavMeshAgent, wait for it to arrive at its destination
            if (navMeshAgent != null)
            {
                // Wait until the path is calculated and the agent arrives within stopping distance
                while (isSequenceRunning)
                {
                    // If no path is pending and we're within stopping distance, consider it reached
                    if (!navMeshAgent.pathPending)
                    {
                        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.01f)
                        {
                            // Either agent has no path or is effectively stopped
                            if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                                break;
                        }
                    }

                    // Fallback: also check player transform proximity (ignore height)
                    if (playerController != null && playerController.transform != null)
                    {
                        var playerPos = playerController.transform.position;
                        var player2 = new Vector2(playerPos.x, playerPos.z);
                        if (Vector2.Distance(player2, target2) <= waypointThreshold)
                            break;
                    }

                    yield return null;
                }
            }
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