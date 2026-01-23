using System;
using Runtime.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Animation
{
    public class SpeechBubble : MonoBehaviour, IPointerClickHandler
    {
        public event Action<int> OptionSelected; 
        
        [AutoAssign] [SerializeField] private BaseTweener tweener;
        [AutoAssign] [SerializeField] private TextMeshProUGUI text;
        
        private int _index;
        
        public void Show(string paragraph, int index)
        {
            _index = index;
            tweener.Play();
            text.text = paragraph;
        }

        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            transform.localScale = Vector3.zero;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OptionSelected?.Invoke(_index);
        }
    }
}