using System.Collections;
using Runtime.Scripts.Interactables;
using UnityEngine;

namespace Runtime.Scripts.Animation
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float duration;
        [SerializeField] private float angle;
        [SerializeField] private Interactable interactable;
        [SerializeField] private BoxCollider _collider;

        [Header("Debug")] 
        [SerializeField] private bool isOpen;
        [SerializeField] private float _progress;
        [SerializeField] private bool _isRunning;


        private void StartRotation()
        {
            if(_isRunning)
                return;
            
            StartCoroutine(DoProgress());
        }
        
        private IEnumerator DoProgress()
        {
            _isRunning = true;
            _collider.enabled = false;
            
            while (_progress < 1f)
            {
                float deltaAngle = Time.deltaTime / duration * angle;
                
                if(isOpen)
                    transform.RotateAround(pivot.position, Vector3.up, deltaAngle);
                
                else
                    transform.RotateAround(pivot.position, Vector3.up, -deltaAngle);
                
                _progress += Time.deltaTime / duration;
                yield return null;
            }

            _progress = 0f;
            isOpen = !isOpen;
            _isRunning = false;
            _collider.enabled = true;
        }

        // private void OnGUI()
        // {
        //     if (GUI.Button(new Rect(10, 10, 100, 30), "Start Rotation"))
        //     {
        //         StartRotation();
        //     }
        // }

        private void Awake()
        {
            interactable.OnInteractionSuccessful += StartRotation;
        }
    }
}