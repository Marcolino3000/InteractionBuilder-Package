using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class Waypoints : MonoBehaviour
    {
        public List<Transform> waypoints;
        public Reaction Reaction;

        private void OnEnable()
        {
            Reaction.SetWaypoints(this);
            
            waypoints = new List<Transform>();
            
            foreach (Transform child in transform)
            {
                waypoints.Add(child);
            }
        }

        public List<Transform> GetWaypoints()
        {
            return waypoints;
        }

        
        
    }
}