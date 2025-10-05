using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private Vector3 _dialogOffset;

        private Vector3 _followOffset;
        private CinemachineFollow _follow;
        private bool _isInDialogMode;
        private float _progress;

        private void Awake()
        {
            _follow = GetComponent<CinemachineFollow>();
            _followOffset = _follow.FollowOffset; 
        }
    
        private void ToggleDialogMode()
        {
            StartCoroutine(DoProgress());
        }

        private IEnumerator DoProgress()
        {
            while (_progress < 1.0f)
            {
                _progress += Time.deltaTime / _duration;
            
                // onValueChanged?.Invoke(_progress);
                TweenUpdate(_ease.Evaluate(_progress));
                yield return _progress;
            }
        
            _progress = 0.0f;
            _isInDialogMode = !_isInDialogMode;
        }

        private void TweenUpdate(float progress)
        {
            _follow.FollowOffset = _isInDialogMode ? 
                Vector3.Lerp(_dialogOffset, _followOffset, progress) : 
                Vector3.Lerp(_followOffset, _dialogOffset, progress);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(500, 10, 140, 30), "Toggle Cam DialogMode"))
            {
                ToggleDialogMode();
            }
        }
    }
}
