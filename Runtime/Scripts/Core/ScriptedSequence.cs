using System;
using System.Collections.Generic;
using Runtime.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    [CreateAssetMenu(menuName = "InteractionBuilder/ScriptedSequence")]
    public class ScriptedSequence : ScriptableObject
    {
        [SerializeReference] public List<Waypoint> waypoints;
        
        public void UpdateTransforms(List<Transform> transforms)
        {
            waypoints ??= new List<Waypoint>();

            foreach (var transform in transforms)
            {
                var id = GlobalObjectId.GetGlobalObjectIdSlow(transform);
                var waypointToUpdate = waypoints.Find(waypoint => waypoint.ID.Equals(id));
                
                if (waypointToUpdate != null)
                {
                    waypointToUpdate.Position = transform.position;
                }                
                else
                {
                    waypoints.Add(new Waypoint(id, transform.position, 0f));
                }
            }
            
            EditorUtility.SetDirty(this);
        }
    }
    
    [Serializable]
    public class Waypoint
    {
        public Waypoint(GlobalObjectId id, Vector3 transform, float waitTime, Reaction reactionAtStart = null, Reaction reactionAtStop = null)
        {
            Position = transform;
            WaitTime = waitTime;
            ReactionAtStart = reactionAtStart;
            ReactionAtStop = reactionAtStop;
        }

        public GlobalObjectId ID;
        public Vector3 Position;
        public float WaitTime;
        public Reaction ReactionAtStart;
        public Reaction ReactionAtStop;
    }
}