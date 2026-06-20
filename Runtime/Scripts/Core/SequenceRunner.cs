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
        [SerializeField] private float waypointStoppingDistance;
        [SerializeField] private bool disableColliderDuringSequence;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private bool useProximityCheck;
        
        [Header("Debug")]
        [SerializeField] private bool logWaypointArrival;
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

            // The agent inherits whatever stoppingDistance the last click left on it (interaction
            // clicks use a larger one). For waypoint movement we want the agent to actually reach
            // each waypoint, so pin the stoppingDistance here instead of trusting leftover state.
            if (navMeshAgent != null)
                navMeshAgent.stoppingDistance = waypointStoppingDistance;

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
            
            if (navMeshAgent == null)
                yield break;

            var target = waypoint.Position;
            var targetXZ = new Vector2(target.x, target.z);

            // Arrival is decided from the agent's own transform position, which is the only
            // value that is never stale. remainingDistance / hasPath / velocity all read their
            // defaults (0 / false) for a frame or two after SetDestination on a fresh agent, so
            // any check based on them can fire instantly before the agent has moved at all (this
            // is what made the first run "skip" movement while a later run, with a warmed-up
            // agent, worked). Distance from the agent to the waypoint cannot lie that way.
            while (isSequenceRunning)
            {
                // Don't evaluate arrival while the path is still being computed.
                if (navMeshAgent.pathPending)
                {
                    yield return null;
                    continue;
                }

                var agentPos = navMeshAgent.transform.position;
                var agentXZ = new Vector2(agentPos.x, agentPos.z);
                var distance = Vector2.Distance(agentXZ, targetXZ);

                // Genuinely standing on the waypoint.
                bool reachedWaypoint = distance <= waypointThreshold;

                // Or the agent has braked to a halt as close as stoppingDistance lets it get.
                bool settledAtStoppingDistance =
                    distance <= navMeshAgent.stoppingDistance + waypointThreshold &&
                    navMeshAgent.velocity.sqrMagnitude <= 0.0001f;

                // Waypoint is unreachable (off-mesh / blocked): the agent has a non-complete path
                // and has come to rest somewhere short of it. Don't hang the sequence forever.
                bool cannotReach =
                    navMeshAgent.hasPath &&
                    navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete &&
                    navMeshAgent.velocity.sqrMagnitude <= 0.0001f;

                if (reachedWaypoint || settledAtStoppingDistance || cannotReach)
                {
                    if (logWaypointArrival)
                        Debug.Log($"[SequenceRunner] Stopped at waypoint, distance {distance:F3} " +
                                  $"(stoppingDistance {navMeshAgent.stoppingDistance:F3}, " +
                                  $"pathStatus {navMeshAgent.pathStatus}, hasPath {navMeshAgent.hasPath}, " +
                                  $"reachedWaypoint {reachedWaypoint}, settled {settledAtStoppingDistance}, " +
                                  $"cannotReach {cannotReach})");

                    if (cannotReach && !reachedWaypoint && !settledAtStoppingDistance)
                        Debug.LogWarning($"[SequenceRunner] Waypoint not reachable; path is " +
                                         $"{navMeshAgent.pathStatus}. Continuing sequence.");
                    break;
                }

                yield return null;
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