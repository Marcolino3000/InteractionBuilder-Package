using System;
using System.Collections.Generic;
using System.Linq;
using Nodes.Decorator;
using Runtime.Scripts.Core;
using Runtime.Scripts.Interactables;
using Tree;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace Editor
{
    public class CompleteInteractionCreator : EditorWindow
    {
        [SerializeField] private string interactionName;
        [SerializeField] private InteractionData interactionData;
        [SerializeField] private InteractableState interactableState;
        [SerializeField] private Reaction successReaction;
        [SerializeField] private Reaction failureReaction;
        [SerializeField] private DialogTree dialogTree;

        [SerializeField] private InteractionData interactionToExecute;
        [SerializeField] private DialogOptionNode dialogOptionToUnlock;
        [SerializeField] private InteractableState triggeringInteractable;
        [SerializeField] private InteractionTriggerVia triggerType;
        [SerializeField] private bool isHighPriority;
        [SerializeField] private List<OwnerWithSettings> conditions = new();

        [SerializeField] private InteractionViewer viewer;
        private GameObject interactablePrefab;
        
        private bool createInteractionWithPrerequisites;
        private List<VisualElement> InteractionPrerequisiteElements = new ();

        private bool createInteractionData;
        private bool createInteractable;
        private bool createDialogTree;
        private bool createSuccessReaction;
        private bool createFailureReaction;

        private SerializedObject editor;
        private VisualElement inspectorContainer;
        private Toggle interactableToggle;
        private Button deleteButton;
        
        private string assetPath = "Packages/com.marc.interactionbuilder/Resources/ScriptableObjects/";
        private string nameOfLastCreatedInteraction;
        private string dropDownValue;
        private string nameOfLastCreatedInteractable;

        [MenuItem("Tools/Interaction Creator")]
        public static void ShowWindow()
        {
            CompleteInteractionCreator wnd = GetWindow<CompleteInteractionCreator>();
            wnd.titleContent = new GUIContent("Interaction Creator");
        }

        private void OnCreateInteractionClicked()
        {
            if (interactionData != null && successReaction != null)
            {
                interactionData.successReaction = successReaction;
            }
            
            if (interactionData != null && failureReaction != null)
            {
                interactionData.failureReaction = failureReaction;
            }
            
            if (successReaction != null && dialogTree != null)
            {
                successReaction.DialogTree = dialogTree;
            }
            
            if (interactionData != null && createInteractionData)
            {
                AssetDatabase.CreateAsset(interactionData, assetPath + "InteractionData/" + interactionName + "InteractionData.asset");
            }
            
            // if (interactableState != null)
            // {
            //     AssetDatabase.CreateAsset(interactableState, assetPath + "Interactables/" + interactionName + "Interactable.asset");
            // }
            
            if (successReaction != null && createSuccessReaction)
            {
                AssetDatabase.CreateAsset(successReaction, assetPath + "Reactions/" + interactionName + "SuccessReaction.asset");
            }
            
            if (failureReaction != null && createFailureReaction)
            {
                AssetDatabase.CreateAsset(failureReaction, assetPath + "Reactions/" + interactionName + "FailureReaction.asset");
            }
            
            if (dialogTree != null && createDialogTree)
            {
                AssetDatabase.CreateAsset(dialogTree, assetPath + "Dialog/" + interactionName + "Dialog.asset");
            }

            if (!createInteractionWithPrerequisites) 
                return;
            
            var record = new PrerequisiteRecord
            {
                InteractionToExecute = interactionToExecute,
                TriggerType = triggerType,
                TriggeringInteractable = triggeringInteractable
            };
            
            if (dialogOptionToUnlock != null)
            {
                dialogOptionToUnlock.IsAvailable = false;
                record.DialogOptionToUnlock = dialogOptionToUnlock;
            }
            
            if(conditions != null)
            {   
                record.Conditions = new List<StateWithSettings>();
                
                foreach (var condition in conditions)
                {
                    record.Conditions.Add(new StateWithSettings()
                    {
                        RecordedState = condition.Owner.CurrentState,
                        IsHardCondition = condition.IsHardCondition
                    });
                }
            }
                
            var trigger = new Trigger(triggerType, triggeringInteractable);

            // viewer.AddInteraction(interactionName, isHighPriority, trigger, record);
            SerializedObject viewerSerializedObject = new SerializedObject(viewer);

            var list = viewerSerializedObject.FindProperty("triggerToPrerequisites");
            var field = new PropertyField();
            field.BindProperty(list);
            list.InsertArrayElementAtIndex(list.arraySize);
            list.GetArrayElementAtIndex(list.arraySize - 1).boxedValue =
                new TriggerToPrereqs(trigger, new List<PrereqNamePriority> { new (record, interactionName, false) });

            viewerSerializedObject.ApplyModifiedProperties();
            viewerSerializedObject.Update();

            EditorUtility.SetDirty(viewer);
            AssetDatabase.SaveAssets();

            nameOfLastCreatedInteraction = interactionName;
            deleteButton.SetEnabled(true);
        }

        private void DeleteNewlyCreatedAssets()
        {
            AssetDatabase.DeleteAsset(assetPath + "InteractionData/" + nameOfLastCreatedInteraction + "InteractionData.asset");
            AssetDatabase.DeleteAsset(assetPath + "Interactables/" + nameOfLastCreatedInteraction + "Interactable.asset");
            AssetDatabase.DeleteAsset(assetPath + "Reactions/" + nameOfLastCreatedInteraction + "SuccessReaction.asset");
            AssetDatabase.DeleteAsset(assetPath + "Reactions/" + nameOfLastCreatedInteraction + "FailureReaction.asset");
            AssetDatabase.DeleteAsset(assetPath + "Dialog/" + nameOfLastCreatedInteraction + "Dialog.asset");
            
            deleteButton.SetEnabled(false);
        }

        #region BuildEditorUI

        public void CreateGUI()
        {
            editor = new SerializedObject(this);
            
            VisualElement root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Row;
            
            minSize = new Vector2(700, 400);
            maxSize = new Vector2(1000, 1000);

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.marginRight = 10;
            
            container.Add(CreateNameField());
            
            container.Add(CreateScriptableObjectFields());

            container.Add(CreateInteractionFields());

            container.Add(CreateCreatorButton());
            
            container.Add(CreateDeleteButton());
            
            root.Add(container);

            root.Add(CreateInspectorContainer(null));
            
            foreach (var element in InteractionPrerequisiteElements)
            {
                element.SetEnabled(false);
            }

            // GetPrefabReferences();
        }

        private void GetPrefabReferences()
        {
            interactablePrefab = AssetDatabase.LoadAssetAtPath("Packages/com.cod.interactionbuilder/Resources/Prefabs/SpriteInteractablePrefab.prefab", typeof(GameObject)) as GameObject;
            var interactable = Instantiate(interactablePrefab);
            interactable.name = interactionName;
        }

        private VisualElement CreateDeleteButton()
        {
            var button = new Button()
            {
                text = "Delete Created Assets",
                style =
                {
                    width = 150,
                    marginTop = 10,
                    alignSelf = Align.Center
                }
            };

            button.clicked += DeleteNewlyCreatedAssets;
            button.SetEnabled(false);
            
            deleteButton = button;
            return button;
        }

        private VisualElement CreateCreatorButton()
        {
            var button = new Button
            {
                text = "Create",
                style =
                {
                    width = 150,
                    marginTop = 10,
                    alignSelf = Align.Center
                }
            };

            button.clicked += () => OnCreateInteractionClicked();
            return button;
        }

        private VisualElement CreateConditionsField()
        {
            var listProp = editor.FindProperty("conditions");
            var listField = new PropertyField(listProp);
            listField.BindProperty(listProp);
            return listField;
            
        }

        private VisualElement CreateInspectorContainer(SerializedProperty property)
        {
            inspectorContainer = new VisualElement();
            inspectorContainer.style.minWidth = 320;
            // inspectorContainer.style.flexGrow = 1;
            inspectorContainer.style.borderBottomColor = new Color(0.8f, 0.8f, 0.8f);
            inspectorContainer.style.borderBottomWidth = 1;
            inspectorContainer.style.borderLeftColor = new Color(0.8f, 0.8f, 0.8f);
            inspectorContainer.style.borderLeftWidth = 1;
            inspectorContainer.style.borderRightColor = new Color(0.8f, 0.8f, 0.8f);
            inspectorContainer.style.borderRightWidth = 1;
            inspectorContainer.style.borderTopColor = new Color(0.8f, 0.8f, 0.8f);
            inspectorContainer.style.borderTopWidth = 1;
            
            inspectorContainer.style.borderBottomLeftRadius = 4;
            inspectorContainer.style.borderBottomRightRadius = 4;
            inspectorContainer.style.borderTopLeftRadius = 4;
            inspectorContainer.style.borderTopRightRadius = 4;

            CreateInspector(property);

            return inspectorContainer;
        }

        private VisualElement CreateInteractionFields()
        {
            var container = new VisualElement();
            
            container.style.width = 276;
            container.style.marginTop = 15;
            
            container.Add(CreateInteractionToggle());
            
            CreateInteractionScriptableObjectFields(container);
            
            container.Add(CreateTriggerTpyeEnumField());
            
            container.Add(CreatePriorityToggleField());
            
            container.Add(CreateConditionsField());
            
            InteractionPrerequisiteElements.AddRange(container.Children().Skip(1).ToList());
            
            return container;
        }

        private VisualElement CreatePriorityToggleField()
        {
            var toggleField = new Toggle("Is High Priority");
            toggleField.BindProperty(editor.FindProperty(nameof(isHighPriority)));
            return toggleField;
        }

        private VisualElement CreateTriggerTpyeEnumField()
        {
            var enumField = new EnumField("Trigger Type", InteractionTriggerVia.ButtonPress);
            enumField.BindProperty(editor.FindProperty(nameof(triggerType)));
            return enumField;
        }

        private void CreateInteractionScriptableObjectFields(VisualElement container)
        {
            container.Add(CreateScriptableObjectFields(
                "Interaction to Execute", null, typeof(InteractionData), nameof(interactionToExecute)));
            
            container.Add(CreateScriptableObjectFields(
                "Triggering Interactable", null, typeof(InteractableState), nameof(triggeringInteractable)));
            
            container.Add(CreateScriptableObjectFields(
                "Dialog Option to Unlock", null, typeof(DialogOptionNode), nameof(dialogOptionToUnlock)));
        }

        private VisualElement CreateInteractionToggle()
        {
            var toggle = new Toggle();
            toggle.text = "Create Interaction with Prerequisites";
            toggle.style.marginBottom = 7;
            
            toggle.RegisterValueChangedCallback(HandleCreatInteractionToggle);
            return toggle;
        }

        private void HandleCreatInteractionToggle(ChangeEvent<bool> evt)
        {
            foreach (var element in InteractionPrerequisiteElements)
            {
                element.SetEnabled(evt.newValue);
            }
            
            createInteractionWithPrerequisites = evt.newValue;
            
            if (!evt.newValue)
                return;
            
            interactionToExecute = interactionData;
            triggeringInteractable = interactableState;
            conditions = new List<OwnerWithSettings>();
        }

        private VisualElement CreateScriptableObjectFields()
        {
            var container = new VisualElement();
            container.style.height = 140;
            container.style.justifyContent = Justify.SpaceBetween;
            
            container.Add(CreateScriptableObjectFields(
                "Create new Interaction", HandleCreateInteractionDataToggled, typeof(InteractionData), nameof(interactionData)));
            
            var interactableField = CreateScriptableObjectFields(
                "Create new", HandleCreateInteractableDataToggled, typeof(InteractableState), nameof(interactableState));

            AddDropDownToToggle(interactableField);

            container.Add(interactableField);
            
            container.Add(CreateScriptableObjectFields(
                "Create new Dialog Tree", HandleCreateDialogTreeToggled, typeof(DialogTree), nameof(dialogTree)));
            
            container.Add(CreateScriptableObjectFields(
                "Create success Reaction", HandleCreateSuccessReactionToggled, typeof(Reaction), nameof(successReaction)));
            
            container.Add(CreateScriptableObjectFields(
                "Create failure Reaction", HandleCreateFailureReactionToggled, typeof(Reaction), nameof(failureReaction)));
            
            return container;
        }

        private void AddDropDownToToggle(VisualElement interactableField)
        {
            interactableToggle = (Toggle) interactableField.hierarchy.ElementAt(0);
            var dropDown = new DropdownField();
            dropDown.choices = GetInteractableTypeNames();
            dropDown.style.width = 87;
            dropDown.style.flexShrink = 0;
            dropDown.RegisterValueChangedCallback(HandleDropDownValueChanged);
            
            interactableToggle.hierarchy.Insert(1, dropDown);
            interactableToggle.hierarchy.ElementAt(0).style.width = 69;
            interactableToggle.hierarchy.ElementAt(0).style.minWidth = 20;
            interactableToggle.hierarchy.ElementAt(0).style.marginRight = 0;
        }

        private void HandleDropDownValueChanged(ChangeEvent<string> evt)
        {
            dropDownValue = evt.newValue;
            interactableToggle.value = false;
            interactableToggle.value = true;
        }

        private VisualElement CreateNameField()
        {
            var nameTextField = new TextField("Interaction Name");
            nameTextField.value = interactionName;
            nameTextField.style.marginBottom = 10;
            nameTextField.style.marginTop = 10;
            nameTextField.style.width = 400;
            
            nameTextField.RegisterValueChangedCallback(evt => interactionName = evt.newValue);
            return nameTextField;
        }

        private VisualElement CreateScriptableObjectFields(string toggleLabel, EventCallback<ChangeEvent<bool>> toggleHandler, Type soType, string soPropertyName)
        {
            var container = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            
            
            if(toggleHandler != null)
            {
                var toggle = CreateFieldToggle(toggleLabel, toggleHandler);
                container.Add(toggle);
                container.Add(CreateSOField(soType, soPropertyName, toggle));
            }

            else
                container.Add(CreateSOField(soType, soPropertyName));

            return container;
        }

        private Toggle CreateFieldToggle(string label, EventCallback<ChangeEvent<bool>> toggleHandler)
        {
            var interactionFieldToggle = new Toggle(label);
            {
                interactionFieldToggle.style.width = 180;
                interactionFieldToggle.style.alignItems = Align.FlexStart;
                interactionFieldToggle.labelElement.style.width = 160;
                interactionFieldToggle.style.marginTop = 5;
            }
            
            interactionFieldToggle.RegisterValueChangedCallback(toggleHandler);
            
            return interactionFieldToggle;
        }

        private ObjectField CreateSOField(Type type, string propertyName, Toggle toggle = null)
        {
            var soField = new ObjectField
            {
                objectType = type,
                style = { width = 270, height = 22, flexGrow = 0f, flexShrink = 0f }
            };
            
            var soProp = editor.FindProperty(propertyName);
            // soField.RegisterValueChangedCallback(evt => CreateInspector(soProp));

            if (toggle != null)
                soField.RegisterValueChangedCallback(evt =>
                {
                    if(CheckIfExistingObjectWasSelected(evt))
                    {
                        toggle.value = false;
                        return;
                    }
                    
                    toggle.value = evt.newValue != null;
                });
            
            soField.BindProperty(soProp);

            soField.RegisterCallback<FocusEvent>((evt => { CreateInspector(soProp); })) ;
            
            return soField;
        }

        private bool CheckIfExistingObjectWasSelected(ChangeEvent<Object> evt)
        {
            return evt.newValue != null && !evt.newValue.name.Contains(interactionName);
        }

        private void CreateInspector(SerializedProperty soProp)
        {
            InspectorElement inspector;
            
            inspector = soProp != null ? new InspectorElement(soProp.objectReferenceValue) : new InspectorElement();
            
            // inspector.BindProperty(soProp);
            inspectorContainer.Clear();
            inspectorContainer.Add(inspector);
        }

        private Foldout CreateInspectorFoldout()
        {
            var foldout = new Foldout()
            {
                text = "Inspector",
                style =
                {
                    marginLeft = 20,
                    // width = 250
                    flexGrow = 1 
                }
            };

            return foldout;
        }
        
        private InspectorElement CreateSoInspector(string propertyName)
        {
            var soProp = editor.FindProperty(propertyName);
            InspectorElement inspectorElement = new InspectorElement(soProp.objectReferenceValue);
            inspectorElement.BindProperty(soProp);

            return inspectorElement;
        }

        private void HandleCreateInteractionDataToggled(ChangeEvent<bool> evt)
        {
            createInteractionData = evt.newValue;
                
            if (evt.newValue)
            {
                // if(interactionData != null && interactionData.name != interactionName + "Interaction")
                //     return;
                
                interactionData = CreateInstance<InteractionData>();
                interactionData.name = interactionName + "Interaction";
            }
            else
            {
                if (interactionData == null)
                    return;
                
                if(interactionData.name == interactionName + "Interaction")
                    interactionData = null;
            }
        }

        private void HandleCreateInteractableDataToggled(ChangeEvent<bool> evt)
        {
            createInteractable = evt.newValue;
            
            if (evt.newValue)
            {
                switch (dropDownValue)
                {
                    case "Interaction":
                        interactableState = CreateInstance<InteractableState>();
                        break;
                    case "Toggleable":
                        interactableState = CreateInstance<Toggleable>();
                        break;
                }
                
                
                AssetDatabase.CreateAsset(interactableState, assetPath + "Interactables/" + interactionName + "Interactable.asset");
                nameOfLastCreatedInteractable = interactionName;
                interactableState.name = interactionName + "Interactable";
            }
            else
            {
                AssetDatabase.DeleteAsset(assetPath + "Interactables/" + nameOfLastCreatedInteractable + "Interactable.asset");
                
                if(interactableState == null)
                    return;

                if(interactableState.name == interactionName)
                    interactableState = null;
            }
        }

        private void HandleCreateDialogTreeToggled(ChangeEvent<bool> evt)
        {
            createDialogTree = evt.newValue;
            
            if (evt.newValue)
            {
                dialogTree = CreateInstance<DialogTree>();
                dialogTree.name = interactionName + "Dialog";
            }
            else
            {
                if(dialogTree == null)
                    return;

                if(dialogTree.name == interactionName + "Dialog")
                    dialogTree = null;
            }
        }

        private void HandleCreateSuccessReactionToggled(ChangeEvent<bool> evt)
        {
            createSuccessReaction = evt.newValue;
            
            if (evt.newValue)
            {
                successReaction = CreateInstance<Reaction>();
                successReaction.name = interactionName + "SuccessReaction";
            }
            else
            {
                if(successReaction == null)
                    return;
                
                if(successReaction.name == interactionName + "SuccessReaction")
                    successReaction = null;
            }
        }
        
        private void HandleCreateFailureReactionToggled(ChangeEvent<bool> evt)
        {
            createFailureReaction = evt.newValue;
            
            if (evt.newValue)
            {
                failureReaction = CreateInstance<Reaction>();
                failureReaction.name = interactionName + "FailureReaction";
            }
            else
            {
                if(failureReaction == null)
                    return;
                
                if(failureReaction.name == interactionName + "FailureReaction")
                    failureReaction = null;
            }
        }
        
        #endregion

        private List<string> GetInteractableTypeNames()
        {
            List<string> names = new();
            
            names.Add("Interaction");
            names.Add("Toggleable");
            
            // var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            // var interactableTypes = assembly.GetTypes().Where(t => typeof(InteractableState).IsAssignableFrom(t));
            // foreach (var type in interactableTypes)
            // {
            //     names.Add(type == typeof(InteractableState) ? "Interactable" : type.ToString());
            // }
            
            return names;

        }
    }
}