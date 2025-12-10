using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class SequenceRunner : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private Waypoints Waypoints;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float waypointThreshold = 0.2f;

        private Coroutine moveCoroutine;
        private List<Reaction> allReactions;

        private void OnEnable()
        {
            allReactions = new List<Reaction>(Resources.FindObjectsOfTypeAll<Reaction>());

            foreach (var reaction in allReactions)
            {
                reaction.OnStartSequence += StartMovingPlayer;
            }
        }
        
        [ContextMenu("start sequence")]
        public void StartMovingPlayer(Waypoints waypoints)
        {
            Waypoints = waypoints;
            
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
            
            moveCoroutine = StartCoroutine(MoveToWaypoints());
        }

        private IEnumerator MoveToWaypoints()
        {
            if (boxCollider != null)
                boxCollider.enabled = false;
            if (Waypoints == null || Waypoints.GetWaypoints() == null || Waypoints.GetWaypoints().Count == 0)
            {
                Debug.LogWarning("No waypoints set.");
                yield break;
            }
            int currentWaypointIndex = 0;
            var waypoints = Waypoints.GetWaypoints();
            while (currentWaypointIndex < waypoints.Count)
            {
                var waypoint = waypoints[currentWaypointIndex];
                var target = waypoint.Transform.position;
                
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
        }
    }
}