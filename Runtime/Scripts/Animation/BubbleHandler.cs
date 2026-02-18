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
        private PlayerDialogOption[] _currentOptions;

        private void Awake()
        {
            _bubbles = GetComponentsInChildren<SpeechBubble>().ToList();
        }

        public void ShowDialogOptions(DialogOptionNode[] options)
        {
            _currentOptions = new PlayerDialogOption[options.Length];
            
            for (int i = 0; i < options.Length; i++)
            {
                if (!(options[i] is PlayerDialogOption playerOption))
                {
                    Debug.LogError($"BubbleHandler: Option at index {i} is not a PlayerDialogOption!");
                    return;
                }
                
                _currentOptions[i] = playerOption;
            }
            
            if (_currentOptions.Length == 1 && _currentOptions[0].Type == AnswerType.SelfTalk)
            {
                OnOptionSelected(0);
                return;
            }
            
            for (int i = 0; i < _bubbles.Count && i < _currentOptions.Length; i++)
            {
                _bubbles[i].gameObject.SetActive(true);
                _bubbles[i].Show(_currentOptions[i].TextPreview, i, _currentOptions[i].Type);
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
                bubble.OptionSelected -= OnOptionSelected;
            }
        }

        public void TriggerIdleReaction()
        {
            throw new NotImplementedException();
        }
    }
}