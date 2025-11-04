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
                var triggerFoldoutContainer = new VisualElement();
                triggerFoldoutContainer.style.flexDirection = FlexDirection.Row;
                
                var triggerContainer = new VisualElement();
                triggerContainer.style.flexDirection = FlexDirection.Column;
                triggerContainer.style.flexGrow = 0f;
                triggerContainer.style.flexShrink = 0f;
                triggerContainer.style.width = 240;
                
                SerializedProperty trigger = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Trigger");
                
                string triggerType = trigger.FindPropertyRelative("TriggerType").enumNames[trigger.FindPropertyRelative("TriggerType").enumValueIndex];
                var triggerTypeLabel = new Label("Trigger Type: " + triggerType);

                var triggeringInteractableField = new ObjectField();
                triggeringInteractableField.BindProperty(trigger.FindPropertyRelative("TriggeringInteractable"));
                
                var triggerFoldout = new Foldout();
                triggerFoldout.text = "Interactions: ";
                triggerFoldout.style.marginLeft = 20;
                
                triggerContainer.Add(triggerTypeLabel);
                triggerContainer.Add(triggeringInteractableField);
                
                triggerFoldoutContainer.Add(triggerContainer);
                triggerFoldoutContainer.Add(triggerFoldout);
                
                root.Add(triggerFoldoutContainer);
                
                SerializedProperty prereqs = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Prerequisites");
                
                for (int j = 0; j < prereqs.arraySize; j++)
                {
                    var row = rowUxml.CloneTree();
                    SerializedProperty prereq = prereqs.GetArrayElementAtIndex(j);

                    var enumField = row.Q<EnumField>();
                    if (enumField != null)
                    {
                        enumField.BindProperty(prereq.FindPropertyRelative("Record").FindPropertyRelative("TriggerType"));
                    }
                    
                    var interactionToExecuteField = row.Q<ObjectField>("interactionToExecute");
                    if (interactionToExecuteField != null)
                    {
                        interactionToExecuteField.BindProperty(prereq.FindPropertyRelative("Record").FindPropertyRelative("InteractionToExecute"));
                    }
                    
                    var conditionsField = row.Q<PropertyField>("conditions");
                    
                    SerializedProperty conditions = prereq.FindPropertyRelative("Record").FindPropertyRelative("Conditions");

                    var condiditonsFoldout = new Foldout();
                    
                    if (conditions.propertyType == SerializedPropertyType.Generic)
                    {
                        
                        // conditionsField.BindProperty(conditions);
                        for(int w = 0; w < conditions.arraySize; w++)
                        {
                            // var obj = conditions.GetArrayElementAtIndex(i).FindPropertyRelative("RecordedState").objectReferenceValue;
                            var obj = conditions.GetArrayElementAtIndex(w);
                            var value = obj.boxedValue;
                            // var hardCondition = obj.FindPropertyRelative("IsHardCondition");
                            var stateWithSettingsField = new PropertyField();
                            stateWithSettingsField.BindProperty(obj);
                            condiditonsFoldout.Add(stateWithSettingsField);
                            
                            var recordedState = obj.FindPropertyRelative("RecordedState");
                            var recordedStateField = new PropertyField(recordedState);
                            recordedStateField.BindProperty(recordedState);
                            condiditonsFoldout.Add(recordedStateField);
                        }
                    }
                    
                    row.Add(condiditonsFoldout);
                    
                    triggerFoldout.Add(row);
                }
                
                root.Add(triggerFoldoutContainer);
            }

            // InspectorElement.FillDefaultInspector(root , serializedObject, this);

            return root ;
        }
    }
}