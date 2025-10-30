// using System;
// using System.Collections.Generic;
// using Nodes.Decorator;
// using Sirenix.OdinInspector;
// using Tree;
// using UnityEngine;
//
// namespace Runtime.Scripts.Core
// {
//     [CreateAssetMenu(menuName = "Dialog Interaction Caller")]
//     public class DialogInteractionCaller : SerializedScriptableObject
//     {
//         [SerializeField] private DialogTreeRunner _dialogTreeRunner;
//         [SerializeField] private Dictionary<DialogOptionNode, InteractionData> optionToInteraction;
//         
//         private void OnEnable()
//         {
//             _dialogTreeRunner.DialogNodeSelected += HandleOptionSelected;
//         }
//
//         private void HandleOptionSelected(DialogOptionNode option)
//         {
//             optionToInteraction[option]?.HandleInteraction();
//         }
//         
//         
//     }
// }