using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class SequenceRunner : MonoBehaviour
    {
        public static event Action<bool> OnSequenceRunningChanged; 
        
        [SerializeField] private bool isSequenceRunning;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private List<Waypoint> Waypoints;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float waypointThreshold = 0.2f;

        private Coroutine moveCoroutine;
        private List<Reaction> allReactions;
        private Action ReactionFinishedCallback;

        private void OnEnable()
        {
            allReactions = new List<Reaction>(Resources.FindObjectsOfTypeAll<Reaction>());

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
            
            moveCoroutine = StartCoroutine(MoveToWaypoints());
            
        }

        private IEnumerator MoveToWaypoints()
        {
            isSequenceRunning = true;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);
            
            if (boxCollider != null)
                boxCollider.enabled = false;
            if (Waypoints == null || Waypoints == null || Waypoints.Count == 0)
            {
                Debug.LogWarning("No waypoints set.");
                yield break;
            }
            int currentWaypointIndex = 0;
            // var waypoints = Waypoints;
            while (currentWaypointIndex < Waypoints.Count)
            {
                var waypoint = Waypoints[currentWaypointIndex];
                var target = waypoint.Position;
                
                if(waypoint.ReactionAtStart != null)
                    waypoint.ReactionAtStart.Execute ();
                
                while (Vector2.Distance(new Vector2(playerController.transform.position.x, playerController.transform.position.z),
                                       new Vector2(target.x, target.z)) > waypointThreshold)
                {
                    Vector3 direction = (target - playerController.transform.position);
                    Vector2 moveInput = new Vector2(direction.x, direction.z).normalized * moveSpeed;
                    playerController.OnMove(moveInput);
                    yield return null;
                }
                
                if(waypoint.ReactionAtStop != null)
                    waypoint.ReactionAtStop.Execute();
                
                if (waypoint.WaitTime > 0f)
                {
                    playerController.OnMove(Vector2.zero);
                    yield return new WaitForSeconds(waypoint.WaitTime);
                }
                
                currentWaypointIndex++;
            }
            
            playerController.OnMove(Vector2.zero);
            
            if (boxCollider != null)
                boxCollider.enabled = true;
            
            ReactionFinishedCallback?.Invoke();
            isSequenceRunning = false;
            OnSequenceRunningChanged?.Invoke(isSequenceRunning);
        }
    }
}