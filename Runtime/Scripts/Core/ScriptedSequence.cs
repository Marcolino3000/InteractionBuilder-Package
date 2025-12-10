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
        public List<Reaction> ReactionsToExecute = new();


        
    }
}