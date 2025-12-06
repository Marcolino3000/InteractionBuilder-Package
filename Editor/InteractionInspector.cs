using System;
using System.Collections.Generic;
using NUnit.Framework;
using Runtime.Scripts.Core;
using Sirenix.Utilities.Editor;
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

        private VisualElement root;
        private VisualElement propertyUpdateContainer;
        private List<Foldout> conditionFoldouts = new();
        
        public override VisualElement CreateInspectorGUI()
        {
            // EditorApplication.projectChanged += TriggerRepaint;
            root = new VisualElement();
            conditionFoldouts.Clear();
            
            var viewer = (InteractionViewer)target;
            
            if (viewer == null)
            {
                Debug.LogWarning("Inspector: interactionHandler was null");
                return null;
            }
            
            root.Add(CreateHandlerObjectField());

            string triggerToPrereqsName = nameof(viewer.triggerToPrerequisites);
            SerializedProperty triggerToPrerequisites = serializedObject.FindProperty(triggerToPrereqsName);
            var triggerToPrereqsField = new PropertyField();
            // triggerToPrereqsField.TrackPropertyValue(triggerToPrerequisites, AddTriggerFields);

            // Add default inspector for debugging
            triggerToPrereqsField.BindProperty(triggerToPrerequisites);
            root.Add(triggerToPrereqsField);
            
            root.Add(AddShowConditionsButton());
            
            propertyUpdateContainer = new VisualElement();
            propertyUpdateContainer.TrackPropertyValue(triggerToPrerequisites, AddTriggerFields);
            root.Add(propertyUpdateContainer);
            
            AddTriggerFields(triggerToPrerequisites);
            
            return root ;
        }

        private Button AddShowConditionsButton()
        {
            
            
            bool isExpanded = false;
            var button = new Button(() =>
            {
                foreach (var foldout in conditionFoldouts)
                {
                    foldout.value = !foldout.value;
                    // foldout.SetValueWithoutNotify(!foldout.value);
                    isExpanded = foldout.value;
                }
            });
            button.text = isExpanded ? "Hide Conditions" : "Show Conditions";
            return button;
        }

        private void AddTriggerFields(SerializedProperty triggerToPrerequisites)
        {
            propertyUpdateContainer.Clear();
            
            for (int i = 0; i < triggerToPrerequisites.arraySize; i++)
            {
                var triggerFoldoutContainer = new VisualElement();
                triggerFoldoutContainer.style.flexDirection = FlexDirection.Row;

                SerializedProperty trigger = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Trigger");

                string triggerType = trigger.FindPropertyRelative("TriggerType").enumNames[trigger.FindPropertyRelative("TriggerType").enumValueIndex];
                var triggerTypeLabel = new Label("Trigger Type: " + triggerType);
                triggerTypeLabel.style.marginBottom = 5;

                var triggeringInteractableField = new ObjectField();
                triggeringInteractableField.BindProperty(trigger.FindPropertyRelative("TriggeringInteractable"));

                var triggerFoldout = new Foldout();
                triggerFoldout.text = "Interactions: ";
                triggerFoldout.style.marginLeft = 20;

                var triggerContainer = CreateTriggerContainer();

                triggerContainer.Add(triggerTypeLabel);
                triggerContainer.Add(triggeringInteractableField);
                
                triggerFoldoutContainer.Add(triggerContainer);
                triggerFoldoutContainer.Add(triggerFoldout);
                
                propertyUpdateContainer.Add(triggerFoldoutContainer);
                
                SerializedProperty prereqs = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Prerequisites");
                
                var viewer = serializedObject.targetObject as InteractionViewer;    
                var triggerToPrereq = viewer.triggerToPrerequisites[i];
                CreatePrerequisiteFields(prereqs, triggerFoldout, triggerToPrereq);
                
                propertyUpdateContainer.Add(triggerFoldoutContainer);
                propertyUpdateContainer.Add(CreateRowSeparator());
            }
        }

        private void CreatePrerequisiteFields(SerializedProperty prereqs, Foldout triggerFoldout,
            TriggerToPrereqs triggerToPrereq)
        {
            for (int j = 0; j < prereqs.arraySize; j++)
            {
                var row = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                    ("Packages/com.cod.interactionbuilder/Editor/CustomInspectors/InteractionHandler/InteractionInspectorRow.uxml").CloneTree();
                row.style.marginBottom = 15;
                foreach (var child  in row.hierarchy.Children())
                {
                    
                    foreach (var ch in child.Children())
                    {
                        Debug.Log(ch.name);    
                    }
                }
                SerializedProperty prereq = prereqs.GetArrayElementAtIndex(j);
                    
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
                //     
                //
                SerializedProperty conditions = prereq.FindPropertyRelative("Record").FindPropertyRelative("Conditions");
                //     
                conditionsField.BindProperty(conditions);
                conditionsField.OnPopulated(() =>
                {
                    Debug.Log("Conditions field populated");
                    var foldout = conditionsField.Q<Foldout>();
                    if (foldout != null)
                    {
                        foldout.value = false;
                        conditionFoldouts.Add(foldout);
                    }
                });

                

                foreach (var child in conditionsField.Children())
                {
                    Debug.Log($"Child element: {child.GetType()} - Name: {child.name}");
                }



                // if (conditions.propertyType == SerializedPropertyType.Generic)
                // {//
                //     for(int w = 0; w < conditions.arraySize; w++)
                //     {
                //         var obj = conditions.GetArrayElementAtIndex(w);
                //         var value = obj.boxedValue;
                //         var stateWithSettingsField = new PropertyField();
                //         stateWithSettingsField.BindProperty(obj);
                //         // conditionsFoldout.Add(stateWithSettingsField);
                //     }
                // }
                // row.Add(conditionsFoldout);
                row.style.flexDirection = FlexDirection.Row;

                // var foldout = conditionsField.Q("unity-list-view__foldout-header");
                // if(foldout is Foldout foldout2)
                //     conditionFoldouts.Add(foldout2);
                
                // CreateActiveAndDeleteButtons(prereqs, row, j, prereq, triggerToPrereq);

                triggerFoldout.Add(row);
                
                foreach (var child in conditionsField.Children())
                {
                    Debug.Log($"Child element: {child.GetType()} - Name: {child.name}");
                }
            }
        }

        private void CreateActiveAndDeleteButtons(SerializedProperty prereqs, TemplateContainer row, int j,
            SerializedProperty prereq, TriggerToPrereqs triggerToPrereq)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    marginLeft = 15,
                    width = 90
                }
            };

            container.Add(AddDeleteButton(j, prereqs, triggerToPrereq));
            container.Add(CreateIsActiveToggle(prereq));
            
            row.Add(container);
        }

        private VisualElement AddIsActiveButton(SerializedProperty prereq)
        {
            var interactionProp = prereq.FindPropertyRelative("Record").FindPropertyRelative("InteractionToExecute");
            if (interactionProp == null || interactionProp.objectReferenceValue == null)
                return new Label("InteractionToExecute is null");

            var interactionDataSO = new SerializedObject(interactionProp.objectReferenceValue);
            var isActiveProp = interactionDataSO.FindProperty("IsActive");
            if (isActiveProp == null)
                return new Label("IsActive is null");

            var isActiveButton = new Button(() =>
            {
                isActiveProp.boolValue = !isActiveProp.boolValue;
                interactionDataSO.ApplyModifiedProperties();
            })
            {
                text = isActiveProp.boolValue ? "Active" : "Inactive"
            };


            return isActiveButton;
        }
        private VisualElement CreateIsActiveToggle(SerializedProperty prereq)
        {
            var interactionProp = prereq.FindPropertyRelative("Record").FindPropertyRelative("InteractionToExecute");
            if (interactionProp == null || interactionProp.objectReferenceValue == null)
                return new Label("InteractionToExecute is null");

            var interactionDataSO = new SerializedObject(interactionProp.objectReferenceValue);
            var isActiveProp = interactionDataSO.FindProperty("IsActive");
            if (isActiveProp == null)
                return new Label("IsActive is null");

            var toggle = new Toggle("Is Active");
            toggle.labelElement.style.minWidth = 10;
            toggle.labelElement.style.marginLeft = 5;
            toggle.labelElement.style.flexGrow = 1;
            toggle.BindProperty(isActiveProp);
            return toggle;
        }

        private VisualElement CreateTriggerContainer()
        {
            var triggerContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 0f,
                    flexShrink = 0f,
                    width = 150,
                    marginLeft = 10f
                }
            };
            
            return triggerContainer;
        }
        private Button AddDeleteButton(int index, SerializedProperty triggerToPrerequisites,
            TriggerToPrereqs triggerToPrereq)
        {
            var prereq = triggerToPrereq.Prerequisites[index];
            
            var currentIndex = index;
            var deleteButton = new Button(() =>
            {
                
                // triggerToPrerequisites.DeleteArrayElementAtIndex(currentIndex);
                // serializedObject.ApplyModifiedProperties();
                var viewer = serializedObject.targetObject as InteractionViewer;
                // var prereq = (PrereqNamePriority) triggerToPrerequisites.GetArrayElementAtIndex(currentIndex).boxedValue;
                // PrerequisiteRecord rec = prereq.Record;
                viewer.DeletePrereq(prereq.Record);
                viewer.DeleteTriggersWithNoPrerequisites();
                serializedObject.ApplyModifiedProperties();
                // Repaint();
                // ForceRedraw();
            });
            
            deleteButton.style.height = 25;
            deleteButton.text = "Delete";

            return deleteButton;
        }
        
        

        // private void TriggerRepaint()
        // {
        //     serializedObject.ApplyModifiedProperties();
        //     EditorUtility.SetDirty(target);
        //     Repaint();
        //     Debug.Log("repaint called");
        // }

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
        
    }
    
   
}

public static class PropertyFieldExtensions
{ 
    public static void OnPopulated(this PropertyField propertyField, Action callback)
    {
        if (propertyField.childCount > 0)
        {
            callback?.Invoke();
        }
        else
        {
            propertyField.schedule.Execute(() => OnPopulated(propertyField, callback));
        }
    }
}