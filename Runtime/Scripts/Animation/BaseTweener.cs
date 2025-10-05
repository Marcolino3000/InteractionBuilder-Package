using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class BaseTweener : MonoBehaviour
    {
        [SerializeField] private float m_duration;
        [SerializeField] private AnimationCurve m_ease = new();
        [SerializeField] private Vector3 m_startingScale;
        [SerializeField] private Vector3 m_targetScale = new(1, 1, 1);
    
        private float m_progress;
        public void Play(Action<float> onValueChanged = null, Action onCompleteCallback = null)
        {
            //Cancel();
            StartCoroutine(DoProgress(onValueChanged, onCompleteCallback));
        }

        private IEnumerator DoProgress(Action<float> onValueChanged = null, Action onCompleteCallback = null)
        {
            transform.localScale = m_startingScale;
        
            while (m_progress < 1.0f)
            {
                m_progress += Time.deltaTime / m_duration;
            
                onValueChanged?.Invoke(m_progress);
                TweenUpdate(m_ease.Evaluate(m_progress));
                yield return m_progress;
            }
        
            m_progress = 0.0f;
            onCompleteCallback?.Invoke();
        }

        private void TweenUpdate(float progress)
        {
            transform.localScale = Vector3.Lerp(m_startingScale, m_targetScale, progress);
            // GetComponent<RectTransform>().localScale = m_startingScale + m_targetScale * progress;
            // GetComponent<CanvasScaler>().scaleFactor = progress;
        }


        private void OnEnable()
        {
            Debug.Assert(m_duration != 0, "Tween-duration may not be equal to 0.0!");
        }
    }
}

