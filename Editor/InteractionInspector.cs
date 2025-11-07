using System;
using System.Collections.Generic;
using Codice.CM.Common.Merge;
using Codice.CM.Triggers;
using Runtime.Scripts.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

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
            
            root.Add(CreateHandlerObjectField());

            string triggerToPrereqsName = nameof(viewer.triggerToPrerequisites);
            SerializedProperty triggerToPrerequisites = serializedObject.FindProperty(triggerToPrereqsName);
            var triggerToPrereqsField = new ObjectField();
            triggerToPrereqsField.BindProperty(triggerToPrerequisites);
            triggerToPrereqsField.RegisterValueChangedCallback(TriggerRepaint);

            for (int i = 0; i < triggerToPrerequisites.arraySize; i++)
            {
                var triggerFoldoutContainer = new VisualElement();
                triggerFoldoutContainer.style.flexDirection = FlexDirection.Row;
                // triggerFoldoutContainer.style.marginBottom = 20;
                
                var triggerContainer = new VisualElement();
                triggerContainer.style.flexDirection = FlexDirection.Column;
                triggerContainer.style.flexGrow = 0f;
                triggerContainer.style.flexShrink = 0f;
                triggerContainer.style.width = 150;
                triggerContainer.style.marginLeft = 10f;
                
                SerializedProperty trigger = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Trigger");
                
                string triggerType = trigger.FindPropertyRelative("TriggerType").enumNames[trigger.FindPropertyRelative("TriggerType").enumValueIndex];
                var triggerTypeLabel = new Label("Trigger Type: " + triggerType);
                triggerTypeLabel.style.marginBottom = 5;

                var triggeringInteractableField = new ObjectField();
                // triggeringInteractableField.style.width = 150;
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
                    var row =AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.marc.interactionbuilder/Editor/CustomInspectors/InteractionHandler/InteractionInspectorRow.uxml").CloneTree();
                    row.style.marginBottom = 15;
                    
                    SerializedProperty prereq = prereqs.GetArrayElementAtIndex(j);

                    // var enumField = row.Q<EnumField>();
                    // if (enumField != null)
                    // {
                    //     enumField.BindProperty(prereq.FindPropertyRelative("Record").FindPropertyRelative("TriggerType"));
                    // }
                    
                    var interactionToExecuteField = row.Q<ObjectField>("interactionToExecute");
                    if (interactionToExecuteField != null)
                    {
                        interactionToExecuteField.BindProperty(prereq.FindPropertyRelative("Record").FindPropertyRelative("InteractionToExecute"));
                    }
                    
                    var optionToUnlock = row.Q<ObjectField>("dialogOptionNode");
                    if (optionToUnlock != null)
                    {
                        optionToUnlock.BindProperty(prereq.FindPropertyRelative("Record").FindPropertyRelative("DialogOptionToUnlock"));
                    }
                    
                    var conditionsField = row.Q<PropertyField>("conditions");
                    
                    SerializedProperty conditions = prereq.FindPropertyRelative("Record").FindPropertyRelative("Conditions");
                    
                    conditionsField.BindProperty(conditions);
                    conditionsField.RegisterValueChangeCallback(TriggerRepaint);

                    var conditionsFoldout = new Foldout();
                    
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
                            conditionsFoldout.Add(stateWithSettingsField);
                            
                            // var recordedState = obj.FindPropertyRelative("RecordedState");
                            // var recordedStateField = new PropertyField(recordedState);
                            // recordedStateField.BindProperty(recordedState);
                            // conditionsFoldout.Add(recordedStateField);
                        }
                    }
                    
                    // row.Add(conditionsFoldout);
                    
                    triggerFoldout.Add(row);
                }
                
                root.Add(triggerFoldoutContainer);
                
                root.Add(CreateRowSeparator());
            }

            // InspectorElement.FillDefaultInspector(root , serializedObject, this);

            return root ;
        }

        private void TriggerRepaint(Object evt)
        {
            // EditorUtility.SetDirty(this);
            // Repaint();
            // serializedObject.Update();
        }

        private VisualElement CreateHandlerObjectField()
        {
            var objectField = new ObjectField();
            objectField.label = "Interaction Handler";
            objectField.objectType = typeof(InteractionHandler);
            objectField.BindProperty(serializedObject.FindProperty("interactionHandler"));
            return objectField;
        }

        private VisualElement CreateRowSeparator()
        {
            var separator = new VisualElement();
            separator.style.height = 4;
            separator.style.marginTop = 10;
            separator.style.marginBottom = 10;
            separator.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            separator.style.borderBottomLeftRadius = 2;
            separator.style.borderBottomRightRadius = 2;
            separator.style.borderTopLeftRadius = 2;
            separator.style.borderTopRightRadius = 2;
            return separator;
        }

//         protected void OnEnable()
//         {
// #if UNITY_2021_1_OR_NEWER
//             UnityEditor.EditorApplication.projectChanged += OnProjectChanged;
// #endif
//         }
//
//         protected void OnDisable()
//         {
// #if UNITY_2021_1_OR_NEWER
//             UnityEditor.EditorApplication.projectChanged -= OnProjectChanged;
// #endif
//         }
//
//         private void OnProjectChanged()
//         {
//             // Repaint if the asset is reimported/changed externally
//             Repaint();
//         }
    }
}