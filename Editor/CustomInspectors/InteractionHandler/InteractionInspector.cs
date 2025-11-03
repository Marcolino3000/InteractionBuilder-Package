using System.Collections.Generic;
using Codice.CM.Common.Merge;
using Codice.CM.Triggers;
using Runtime.Scripts.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.CustomInspectors
{
    [CustomEditor(typeof(InteractionViewer), true)]
    public class InteractionInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset rowUxml;
    
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var viewer = (InteractionViewer)target;

            if (viewer == null)
            {
                Debug.LogWarning("Inspector: interactionHandler was null");
                return null;
            }
            
            
            SerializedProperty triggerToPrerequisites = serializedObject.FindProperty("triggerToPrerequisites");

            for (int i = 0; i < triggerToPrerequisites.arraySize; i++)
            {
                SerializedProperty prereqs = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Prerequisites");
                
                for (int j = 0; j < prereqs.arraySize; j++)
                {
                    var row = rowUxml.CloneTree();
                    SerializedProperty prereq = prereqs.GetArrayElementAtIndex(j);

                    var enumField = row.Q<EnumField>();
                    enumField.BindProperty(prereq.FindPropertyRelative("TriggerType"));
                    
                    var interactionToExecuteField = row.Q<ObjectField>("interactionToExecute");
                    interactionToExecuteField.BindProperty(prereq.FindPropertyRelative("InteractionToExecute"));

                    Debug.Log(triggerToPrerequisites.arraySize);
                    
                    root.Add(row);
                }
            }

            // foreach (var trigger in viewer.triggerToPrerequisites)
            // {
            //     foreach (var prereq in trigger.Prerequisites)
            //     {
            //         var row = rowUxml.CloneTree();
            //         var enumField = row.Q<EnumField>();
            //
            //         SerializedProperty dic = serializedObject.FindProperty("triggerToPrerequisites").FindPropertyRelative();
            //         
            //     }
            // }

            InspectorElement.FillDefaultInspector(root , serializedObject, this);

            return root ;
        }
    }
}