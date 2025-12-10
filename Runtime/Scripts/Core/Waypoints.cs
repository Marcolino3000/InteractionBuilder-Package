using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class Waypoints : MonoBehaviour
    {
        public List<Waypoint> waypoints;
        public Reaction TriggeringReaction;

        private void OnEnable()
        {
            TriggeringReaction.SetWaypoints(this);
        }
        
        [ContextMenu("Set Waypoints")]
        private void Setup()
        {
            waypoints = new List<Waypoint>();

            foreach (Transform child in transform)
            {
                waypoints.Add(new Waypoint(child, 0f));
            }
        }

        public List<Waypoint> GetWaypoints()
        {
            return waypoints;
        }

        [Serializable]
        public struct Waypoint
        {
            public Waypoint(Transform transform, float waitTime, Reaction reactionAtStart = null, Reaction reactionAtStop = null)
            {
                Transform = transform;
                WaitTime = waitTime;
                ReactionAtStart = reactionAtStart;
                ReactionAtStop = reactionAtStop;
            }
            public Transform Transform;
            public float WaitTime;
            public Reaction ReactionAtStart;
            public Reaction ReactionAtStop;
        }
    }
}