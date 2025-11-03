using System.Collections.Generic;
using Codice.CM.Triggers;
using Runtime.Scripts.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.CustomInspectors
{
    [CustomEditor(typeof(InteractionHandler), true)]
    public class InteractionInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset rowUxml;
        
    
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            InteractionHandler handler = (InteractionHandler)target;

            if (handler == null)
            {
                Debug.LogWarning("Inspector: interactionHandler was null");
                return null;
            }

            // foreach (var trigger in handler.triggersToPrerequisitesLowPrio)                
            // {
            //     foreach (var prereq in trigger.Value)
            //     {
            //         var row = rowUxml.CloneTree();
            //         var enumField = row.Q<EnumField>();
            //         // enumField.BindProperty(new SerializedObject(prereq))
            //         SerializedProperty dic = serializedObject.FindProperty("triggersToPrerequisitesLowPrio");
            //     }
            // }
            //
            InspectorElement.FillDefaultInspector(root , serializedObject, this);
        
            return root ;
        }
    }
}