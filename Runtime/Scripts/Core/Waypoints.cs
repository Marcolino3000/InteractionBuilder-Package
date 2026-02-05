using System.Collections.Generic;
using Runtime.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Runtime.Scripts.Core
{
    public class Waypoints : MonoBehaviour
    {
        [InspectorButton("UpdateTransforms")] 
        public bool _UpdateTransforms;
        
        public List<Transform> waypoints;
        public ScriptedSequence sequence;
        
        // private void OnEnable()
        // {
        //     UpdateSequene();
        // }
        //
        // private void OnValidate()
        // {
        //     UpdateSequene();
        // }
        
        
        private void UpdateTransforms()
        {
            if (sequence != null)
            {
                sequence.UpdateTransforms(waypoints);
                EditorUtility.SetDirty(sequence);
            }
            
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