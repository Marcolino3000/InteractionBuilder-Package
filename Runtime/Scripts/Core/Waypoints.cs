using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class Waypoints : MonoBehaviour
    {
        public List<Transform> waypoints;
        public ScriptedSequence sequence;
        private void OnEnable()
        {
            UpdateSequene();
        }

        private void OnValidate()
        {
            UpdateSequene();
        }

        private void UpdateSequene()
        {
            if (sequence != null)
                sequence.UpdateTransforms(waypoints);
        }

        [ContextMenu("Set Waypoints")]
        private void Setup()
        {
            waypoints = new List<Transform>();

            foreach (Transform child in transform)
            {
                waypoints.Add(child);
            }
        }

  

     
    }
}