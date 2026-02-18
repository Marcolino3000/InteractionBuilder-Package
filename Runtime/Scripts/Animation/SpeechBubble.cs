using System;
using System.Collections.Generic;
using Nodes.Decorator;
using Runtime.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace Runtime.Scripts.Animation
{
    public class SpeechBubble : MonoBehaviour, IPointerClickHandler
    {
        public event Action<int> OptionSelected; 
        
        [AutoAssign] [SerializeField] private BaseTweener tweener;
        [AutoAssign] [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private List<Sprite> images;
        
        private int _index;
        
        public void Show(string paragraph, int index)
        {
            _index = index;
            tweener.Play();
            text.text = paragraph;
        }
        
        public void Show(string paragraph, int index, AnswerType answerType)
        {
            _index = index;
            tweener.Play();
            text.text = paragraph;
            image.sprite = images[(int)answerType];
        }

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            transform.localScale = Vector3.zero;
            image.alphaHitTestMinimumThreshold = 0.3f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OptionSelected?.Invoke(_index);
        }
    }
}