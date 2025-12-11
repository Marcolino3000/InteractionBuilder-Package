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
        public Waypoints Waypoints;
        public List<Waypoint> waypoints;
        
        public void UpdateTransforms(List<Transform> transforms)
        {
            waypoints ??= new List<Waypoint>();

            foreach (var transform in transforms)
            {
                var waypointToUpdate = waypoints.Find(waypoint => waypoint.Transform == transform);
                
                if (waypointToUpdate != null)
                {
                    waypointToUpdate.Transform = transform;
                }                
                else
                {
                    waypoints.Add(new Waypoint(transform, 0f));
                }
            }
        }
    }
    
    [Serializable]
    public class Waypoint
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