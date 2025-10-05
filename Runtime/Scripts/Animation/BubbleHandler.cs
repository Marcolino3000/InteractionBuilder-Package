using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Nodes.Decorator;
using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class BubbleHandler : MonoBehaviour, IDialogOptionReceiver
    {
        public event Action<DialogOptionNode> DialogOptionSelected;
        public DialogOptionType DialogOptionType => DialogOptionType.Player;

        private List<SpeechBubble> _bubbles;
        private DialogOptionNode[] _currentOptions;

        private void Awake()
        {
            _bubbles = GetComponentsInChildren<SpeechBubble>().ToList();
        }

        public void ShowDialogOptions(DialogOptionNode[] options)
        {
            _currentOptions = options;
            
            Debug.Log("show dialog options");
            
            for (int i = 0; i < _bubbles.Count && i < options.Length; i++)
            {
                _bubbles[i].gameObject.SetActive(true);
                _bubbles[i].Show(options[i].TextPreview, i);
                _bubbles[i].OptionSelected += OnOptionSelected;
            }
        }

        private void OnOptionSelected(int index)
        {
            DialogOptionSelected?.Invoke(_currentOptions[index]);
            HideDialogOptions();
        }

        public void HideDialogOptions()
        {
            foreach (var bubble in _bubbles)
            {
                bubble.gameObject.SetActive(false);
            }
        }

        public void TriggerIdleReaction()
        {
            throw new NotImplementedException();
        }
    }
}