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

        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private bool isDialogRunning;
        [SerializeField] private TransparentWall transparentWall;
        [SerializeField] private bool playerIsInside;

        private Camera cam;
        private bool clickedWall;

        private void OnEnable()
        {
            cam = Camera.main;
            
            DialogTreeRunner.OnDialogRunningStatusChanged += status =>
            {
                isDialogRunning = status;
            };
            
            transparentWall.OnPlayerTrigger += (wall) =>
            {
                 playerIsInside = !playerIsInside;
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
            var hits = GetAllSortedRaycastHits();
            
            foreach (var hit in hits)
            {
                if (hit.interactable == null)
                {
                    continue;
                }
                
                if (!sauerteig.IsUnlocked)
                    return;

                if (hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                    continue;

                if(sauerteig.awarenessLevel < hit.interactable.Data.AwarenessLevel)
                    continue;

                hit.display?.TriggerHoverEffect();
            }
        }

        private void HandleClickOnInteractables()
        {
            var hits = GetAllSortedRaycastHits();

            Debug.Log("//////HITS///////////////////");
            
            foreach (var hit in hits)
            {
                Debug.Log(hit.target.name + ": " + hit.distance);
            }

            if (!playerIsInside && (hits.Count == 0 || hits[0].target == null || hits[0].target.name == "Wall" || hits[0].interactable == null))
            {
                Debug.Log("click raycast early return");
                return;
            }

            foreach (var hit in hits)
            {
                if(hit.target.name == "Wall")
                    continue;
                
                if (hit.interactable.Found)
                    continue;

                hit.interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, hit.interactable.Data);

                if (sauerteig.awarenessLevel >= hit.interactable.Data.AwarenessLevel &&
                    hit.interactable.Data.AwarenessLevel != AwarenessLevel.NotSet)
                {
                    sauerteig.HandleInteractableDiscovered(hit.interactable.Data);

                    if(hit.interactable.MarkAsFoundWhenClicked)
                        hit.interactable.Found = true;
                }

                //only process the interactable closest to camera
                return;
            }
        }

        private struct RaycastInteractableHit
        {
            public Interactable interactable;
            public InteractableDisplay display;
            public float distance;
            public GameObject target;
            public RaycastInteractableHit(Interactable interactable, InteractableDisplay display, float distance, GameObject target)
            {
                this.interactable = interactable;
                this.display = display;
                this.distance = distance;
                this.target = target;
            }
        }

        private bool GetHitsFrom3DRaycast(List<RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 6);
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                // if(interactable == null || display == null)
                //     continue;
                interactables.Add(new RaycastInteractableHit(interactable, display, hit.distance, hit.collider.gameObject));
            }
            return interactables.Count > 0;
        }

        private bool GetHitsFrom2DRaycast(List<RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                // if(interactable == null || display == null)
                //     continue;
                interactables.Add(new RaycastInteractableHit(interactable, display, hit.distance, hitCollider.gameObject));
            }
            return interactables.Count > 0;
        }

        private List<RaycastInteractableHit> GetAllSortedRaycastHits()
        {
            var interactables3D = new List<RaycastInteractableHit>();
            var interactables2D = new List<RaycastInteractableHit>();
            GetHitsFrom3DRaycast(interactables3D);
            GetHitsFrom2DRaycast(interactables2D);
            var allInteractables = new List<RaycastInteractableHit>(interactables3D);
            allInteractables.AddRange(interactables2D);
            allInteractables.Sort((a, b) => a.distance.CompareTo(b.distance));
            return allInteractables;
        }
    }
}