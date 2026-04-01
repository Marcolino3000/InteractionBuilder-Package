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
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;

        [Header("Debug")] 
        [SerializeField] private bool isOpen;
        [SerializeField] private float _progress;
        [SerializeField] private bool _isRunning;


        private void StartRotation()
        {
            if(_isRunning)
            {
                Debug.LogWarning("Door was already rotating. Ignoring new rotation request.");
                return;
            }
            
            PlayAudio();
            StartCoroutine(DoProgress());
        }

        private void PlayAudio()
        {
            audioSource.PlayOneShot(isOpen ? closeSound : openSound);
        }

        private IEnumerator DoProgress()
        {
            _isRunning = true;
            
            if(_collider != null)
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
            
            if(_collider != null)
                _collider.enabled = true;
        }
        
        private void Awake()
        {
            interactable.OnInteractionSuccessful += StartRotation;
        }
    }
}