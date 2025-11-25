using System;
using System.Collections.Generic;
using NUnit.Framework;
using Runtime.Scripts.Core;
using Sirenix.Utilities;
using Tree;
using Unity.Cinemachine;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Raycaster : MonoBehaviour
    {
        [SerializeField] private DialogTreeRunner dialogTreeRunner;
        
        private Camera cam;
        private Sauerteig sauerteig;
        [SerializeField] private bool isDialogRunning;

        private void OnEnable()
        {
            cam = Camera.main;
            sauerteig = GetComponent<Sauerteig>();
            dialogTreeRunner.OnDialogRunningStatusChanged += status =>
            {
                isDialogRunning = status;
            };
        }

        void Update()
        {
            HandleHoverOnInteractables();
        }

        public void HandleMouseClick()
        {
            if(isDialogRunning)
                return;
            
            HandleClickOnInteractables();
        }

        private void HandleHoverOnInteractables()
        {
            var interactables = new List<Tuple<Interactable, InteractableDisplay>>();

            if (GetHitsFrom3DRaycast(interactables) || GetHitsFrom2DRaycast(interactables))
            {
                foreach (var (interactable, display) in interactables)
                {
                    // Debug.Log(interactable.gameObject.name);
                    
                    if (interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                        continue;

                    display?.TriggerHoverEffect();
                }
            }
        }

        private void HandleClickOnInteractables()
        {
            var interactables = new List<Tuple<Interactable, InteractableDisplay>>();
            
            if(GetHitsFrom3DRaycast(interactables) || GetHitsFrom2DRaycast(interactables))
            {
                foreach (var (interactable, display) in interactables)
                {
                    // Debug.Log(interactable.Data.name);
                    
                    if (interactable.Found)
                        continue;
                    
                    interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, interactable.Data);

                    if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel &&
                        interactable.Data.AwarenessLevel != AwarenessLevel.NotSet)
                    {
                        sauerteig.HandleInteractableDiscovered(interactable.Data);
                        interactable.Found = true;
                    }

                    //only process the interactable closest to camera
                    return;
                }
            }
        }

        private bool GetHitsFrom3DRaycast(List<Tuple<Interactable, InteractableDisplay>> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 6);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
            
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                
                if(interactable == null || display == null)
                {
                    Debug.LogWarning("Interactable or Display was null - ignoring Raycast hit.");
                    continue;
                }
                
                interactables.Add(new Tuple<Interactable, InteractableDisplay>(interactable, display));
            }

            return interactables.Count > 0;
        }

        private bool GetHitsFrom2DRaycast(List<Tuple<Interactable, InteractableDisplay>> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            var hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);

            // interactables = new List<Tuple<Interactable, InteractableDisplay>>();
            
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
            
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                
                if(interactable == null || display == null)
                {
                    Debug.LogWarning("Interactable or Display was null - ignoring Raycast hit.");
                    continue;
                }
                
                interactables.Add(new Tuple<Interactable, InteractableDisplay>(interactable, display));
            }

            return interactables.Count > 0;
        }
    }
}