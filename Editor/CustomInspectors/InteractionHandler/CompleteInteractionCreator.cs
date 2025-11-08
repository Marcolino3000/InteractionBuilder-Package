using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Scripts.Core;
using Runtime.Scripts.Interactables;
using Tree;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public class CompleteInteractionCreator : EditorWindow
    {
        private SerializedObject editor;
        
        [SerializeField] private string interactionName;
        [SerializeField] private InteractionData interactionData;
        [SerializeField] private InteractableState interactableState;
        [SerializeField] private Reaction successReaction;
        [SerializeField] private Reaction failureReaction;
        [SerializeField] private DialogTree dialogTree;

        [SerializeField] private InteractionData interactionToExecute;
        [SerializeField] private InteractableState triggeringInteractable;
        [SerializeField] private InteractionTriggerVia triggerType;
        [SerializeField] private bool isHighPriority;
        
        [SerializeField] private bool createInteractionWithPrerequisites;
        [SerializeField] private List<VisualElement> InteractionPrerequisiteElements = new ();
        
        [SerializeField] private InteractionViewer viewer;
        private string assetPath = "Packages/com.marc.interactionbuilder/Resources/ScriptableObjects/";

        [MenuItem("Tools/Interaction Creator")]
        public static void ShowExample()
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
            
            if (interactionData != null)
            {
                AssetDatabase.CreateAsset(interactionData, assetPath + "InteractionData/" + interactionName + "Data.asset");
            }
            
            if (interactableState != null)
            {
                AssetDatabase.CreateAsset(interactableState, assetPath + "Interactables/" + interactionName + "Interactable.asset");
            }
            
            if (successReaction != null)
            {
                AssetDatabase.CreateAsset(successReaction, assetPath + "Reactions/" + interactionName + "SuccessReaction.asset");
            }
            
            if (failureReaction != null)
            {
                AssetDatabase.CreateAsset(failureReaction, assetPath + "Reactions/" + interactionName + "FailureReaction.asset");
            }
            
            if (dialogTree != null)
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
                
            Trigger trigger = new Trigger(triggerType, triggeringInteractable);
                
            viewer.AddInteraction(interactionName, isHighPriority, trigger, record);
        }

        #region BuildEditorUI
        public void CreateGUI()
        {
            editor = new SerializedObject(this);
            
            VisualElement root = rootVisualElement;

            root.Add(CreateNameField());
            
            CreateScriptableObjectFields(root);
            
            root.Add(CreateInteractionFields());

            root.Add(CreateCreatorButton());
        }

        private VisualElement CreateInteractionFields()
        {
            var container = new VisualElement();
            
            container.style.width = 276;
            
            container.Add(CreateInteractionToggle());
            
            CreateInteractionScriptableObjectFields(container);
            
            container.Add(CreateTriggerTpyeEnumField());
            
            container.Add(CreatePriorityToggleField());
            
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

        private void CreateInteractionScriptableObjectFields(VisualElement root)
        {
            root.Add(CreateScriptableObjectFields(
                "Interaction to Execute", null, typeof(InteractionData), nameof(interactionToExecute)));
            
            root.Add(CreateScriptableObjectFields(
                "Triggering Interactable", null, typeof(InteractableState), nameof(triggeringInteractable)));
        }

        private VisualElement CreateInteractionToggle()
        {
            var toggle = new Toggle();
            toggle.text = "Create Interaction with Prerequisites";
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
        }

        private void CreateScriptableObjectFields(VisualElement root)
        {
            root.Add(CreateScriptableObjectFields(
                "Create new Interaction", HandleCreateInteractionDataToggled, typeof(InteractionData), nameof(interactionData)));
            
            root.Add(CreateScriptableObjectFields(
                "Create new Interactable", HandleCreateInteractableDataToggled, typeof(InteractableState), nameof(interactableState)));
            
            root.Add(CreateScriptableObjectFields(
                "Create new Dialog Tree", HandleCreateDialogTreeToggled, typeof(DialogTree), nameof(dialogTree)));
            
            root.Add(CreateScriptableObjectFields(
                "Create success Reaction", HandleCreateReactionToggled, typeof(Reaction), nameof(successReaction)));
            
            root.Add(CreateScriptableObjectFields(
                "Create failure Reaction", HandleCreateReactionToggled, typeof(Reaction), nameof(failureReaction)));
        }

        private VisualElement CreateNameField()
        {
            var nameTextField = new TextField("Interaction Name");
            nameTextField.value = interactionName;
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

            Foldout inspectorFoldout = null;

            if(toggleHandler != null)
            {
                container.Add(CreateFieldToggle(toggleLabel, toggleHandler));
                inspectorFoldout = CreateInspectorFoldout();
            }

            container.Add(CreateSOField(soType, soPropertyName, inspectorFoldout));

            if(inspectorFoldout != null)
            {
                container.Add(inspectorFoldout);
            }

            return container;
        }

        private VisualElement CreateCreatorButton()
        {
            var button = new Button{text = "Create Interaction"};
            button.clicked += () => OnCreateInteractionClicked();
            return button;
        }

        private Toggle CreateFieldToggle(string label, EventCallback<ChangeEvent<bool>> toggleHandler)
        {
            var interactionFieldToggle = new Toggle(label);
            {
                interactionFieldToggle.style.width = 160;
                interactionFieldToggle.style.alignItems = Align.FlexStart;
                interactionFieldToggle.labelElement.style.width = 140;
                interactionFieldToggle.style.marginTop = 5;
            }
            
            interactionFieldToggle.RegisterValueChangedCallback(toggleHandler);
            
            return interactionFieldToggle;
        }

        private ObjectField CreateSOField(Type type, string propertyName, Foldout inspectorFoldout)
        {
            var interactionField = new ObjectField
            {
                objectType = type,
                style = { width = 270, height = 22, flexGrow = 0f, flexShrink = 0f }
            };
            
            var soProp = editor.FindProperty(propertyName);
            
            interactionField.RegisterValueChangedCallback(evt => HandleSoFieldChanged(type, evt, inspectorFoldout, propertyName));
            
            interactionField.BindProperty(soProp);
            
            return interactionField;
        }

        private Foldout CreateInspectorFoldout()
        {
            var foldout = new Foldout()
            {
                text = "Inspector",
                style =
                {
                    marginLeft = 20,
                    width = 250
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
            if (evt.newValue)
            {
                interactionData = CreateInstance<InteractionData>();
                interactionData.name = interactionName + "Interaction";
            }
            else
            {
                if(interactionData.name == interactionName + "Interaction")
                    interactionData = null;
            }
        }

        private void HandleCreateInteractableDataToggled(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                interactableState = CreateInstance<InteractableState>();
                interactableState.name = interactionName;
            }
            else
            {
                if(interactableState.name == interactionName)
                    interactableState = null;
            }
        }

        private void HandleCreateDialogTreeToggled(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                dialogTree = CreateInstance<DialogTree>();
                dialogTree.name = interactionName + "Dialog";
            }
            else
            {
                if(dialogTree.name == interactionName + "Dialog")
                    dialogTree = null;
            }
        }

        private void HandleCreateReactionToggled(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                successReaction = CreateInstance<Reaction>();
                successReaction.name = interactionName + "Reaction";
            }
            else
            {
                if(successReaction.name == interactionName + "Reaction")
                    successReaction = null;
            }
        }

        private void HandleSoFieldChanged(Type type, ChangeEvent<Object> evt, Foldout inspectorFoldout,
            string propertyName)
        {
            if (evt.newValue == null)
            {
                if (type == typeof(InteractionData))
                    interactionData = null;
                else if (type == typeof(InteractableState))
                    interactableState = null;
                else if (type == typeof(DialogTree))
                    dialogTree = null;
                else if (type == typeof(Reaction))
                    successReaction = null;
                return;
            }

            switch (evt.newValue)
            {
                case InteractionData data:
                    interactionData = data;
                    break;
                case InteractableState state:
                    interactableState = state;
                    break;
                case DialogTree tree:
                    dialogTree = tree;
                    break;
                case Reaction react:
                    successReaction = react;
                    break;
            }

            
            if(inspectorFoldout != null)
            {
                inspectorFoldout.Clear();
                inspectorFoldout.Add(CreateSoInspector(propertyName));
                inspectorFoldout.value = false;
            }

            // EditorUtility.SetDirty(interactionData);
            // editor.ApplyModifiedProperties();
            // Repaint();
        }

        #endregion
    }
}