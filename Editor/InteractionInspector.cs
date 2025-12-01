using Runtime.Scripts.Core;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.CustomInspectors
{
    [CustomEditor(typeof(InteractionViewer), true)]
    public class 
        InteractionInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset rowUxml;

        private VisualElement root;

        public override VisualElement CreateInspectorGUI()
        {
            EditorApplication.projectChanged += TriggerRepaint;
            root = new VisualElement();

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
            triggerToPrereqsField.BindProperty(triggerToPrerequisites);
            
            // Add default inspector for debugging
            root.Add(triggerToPrereqsField);
            
            AddTriggerFields(triggerToPrerequisites);
            
            return root ;
        }

        private void AddTriggerFields(SerializedProperty triggerToPrerequisites)
        {
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
                // triggerContainer.Add(AddDeleteButton(i, triggerToPrerequisites));
                
                triggerFoldoutContainer.Add(triggerContainer);
                triggerFoldoutContainer.Add(triggerFoldout);
                
                root.Add(triggerFoldoutContainer);
                
                SerializedProperty prereqs = triggerToPrerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("Prerequisites");
                
                CreatePrerequisiteFields(prereqs, triggerFoldout);
                
                root.Add(triggerFoldoutContainer);
                root.Add(CreateRowSeparator());
            }
        }

        private void CreatePrerequisiteFields(SerializedProperty prereqs, Foldout triggerFoldout)
        {
            for (int j = 0; j < prereqs.arraySize; j++)
            {
                var row = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                    ("Packages/com.cod.interactionbuilder/Editor/CustomInspectors/InteractionHandler/InteractionInspectorRow.uxml").CloneTree();
                row.style.marginBottom = 15;
                    
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
                    
                SerializedProperty conditions = prereq.FindPropertyRelative("Record").FindPropertyRelative("Conditions");
                    
                conditionsField.BindProperty(conditions);

                var conditionsFoldout = new Foldout();
                    
                if (conditions.propertyType == SerializedPropertyType.Generic)
                {
                        
                    for(int w = 0; w < conditions.arraySize; w++)
                    {
                        var obj = conditions.GetArrayElementAtIndex(w);
                        var value = obj.boxedValue;
                        var stateWithSettingsField = new PropertyField();
                        stateWithSettingsField.BindProperty(obj);
                        conditionsFoldout.Add(stateWithSettingsField);
                    }
                }
                row.style.flexDirection = FlexDirection.Row;
                
                row.Add(AddDeleteButton(j, prereqs));

                triggerFoldout.Add(row);
            }
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

        private Button AddDeleteButton(int index, SerializedProperty triggerToPrerequisites)
        {
            var currentIndex = index;
            var deleteButton = new Button(() =>
            {
                triggerToPrerequisites.DeleteArrayElementAtIndex(currentIndex);
                serializedObject.ApplyModifiedProperties();
                var viewer = serializedObject.targetObject as InteractionViewer;
                viewer?.DeleteTriggersWithNoPrerequisites();
                
                // ForceRedraw();
            });
            
            deleteButton.style.height = 25;
            deleteButton.style.marginLeft = 15;
            deleteButton.text = "Delete";

            return deleteButton;
        }

        private void ForceRedraw()
        {
            root.Clear();
            var newInspector = CreateInspectorGUI();
            // root.Add(newInspector);
        }

        private void TriggerRepaint()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            Repaint();
            // Debug.Log("repaint called");
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
    }
}