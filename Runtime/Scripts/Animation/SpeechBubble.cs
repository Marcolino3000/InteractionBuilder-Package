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
    public class SpeechBubble : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<int> OptionSelected; 
        
        [AutoAssign] [SerializeField] private BaseTweener tweener;
        [AutoAssign] [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private List<Sprite> standardBubbles;
        [SerializeField] private List<Sprite> hoveredBubbles;
        
        private int _index;
        private int _currentAnswerType;
        
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
            _currentAnswerType = (int)answerType;
            image.sprite = standardBubbles[_currentAnswerType];
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.sprite = hoveredBubbles[_currentAnswerType];
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.sprite = standardBubbles[_currentAnswerType];
        }
    }
}