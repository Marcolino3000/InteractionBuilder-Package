using System;
using UnityEngine;

namespace Runtime.Scripts.Utility
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoAssignAttribute : PropertyAttribute
    {
        public bool IncludeInactive { get; set; } = false;
    
        public AutoAssignAttribute(bool includeInactive = false)
        {
            IncludeInactive = includeInactive;
        }
    }
}