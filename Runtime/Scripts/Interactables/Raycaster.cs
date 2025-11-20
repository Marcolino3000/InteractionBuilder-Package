using System;
using System.Collections.Generic;
using Runtime.Scripts.Core;
using Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Runtime.Scripts.Interactables
{
    public class Raycaster : MonoBehaviour
    {
        [SerializeField] private DialogTreeRunner dialogTreeRunner;
        
        private Camera cam;
        private Sauerteig sauerteig;
        [SerializeField] private bool isDialogRunning;
        // [SerializeField] private float cooldownBetweenHovers;

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
            DoHoverRaycast();
        }

        private void DoHoverRaycast()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            
            foreach (var hit in hits)
            {
                var col = hit.collider;
                if (col is MeshCollider meshCollider)
                {
                    var display = meshCollider.gameObject.GetComponentInChildren<InteractableDisplay>();

                    var interactable = meshCollider.gameObject.GetComponentInChildren<Interactable>();

                    if(interactable == null)
                        continue;
                    
                    if(interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                        continue;
                    
                    if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel)
                    {
                        display.TriggerHoverEffect();
                    }
                    
                }
            }
            
            var hits2D = Physics2D.RaycastAll(ray.origin, ray.direction);
            foreach (var hit in hits2D)
            {
                var col = hit.collider;
                if (col is PolygonCollider2D polygonCollider)
                {
                    var display = polygonCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                    var interactable = polygonCollider.gameObject.GetComponentInChildren<Interactable>();
                    
                    if(interactable == null)
                        continue;
                    
                    if(interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                        continue;
                    
                    display?.TriggerHoverEffect();
                }
            }
        }

        public void HandleMouseClick()
        {
            // if(dialogTreeRunner.IsDialogRunning)
            if(isDialogRunning)
                return;
            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Do3DRaycast(ray);
            Do2DRaycast(ray);


            //     Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 6))
        //     {
        //         var col = hit.collider;
        //
        //         var interactable = col.gameObject.GetComponentInChildren<Interactable>();
        //
        //         if (interactable == null)
        //             interactable = col.gameObject.GetComponentInParent<Interactable>();
        //
        //         var display = col.gameObject.GetComponentInChildren<InteractableDisplay>();
        //
        //         if (interactable == null)
        //             return;
        //
        //         if (interactable.Found)
        //             return;
        //
        //         interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, interactable.Data);
        //
        //         if (display == null)
        //             return;
        //
        //         if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel)
        //         {
        //             sauerteig.HandleInteractableDiscovered(interactable.Data);
        //             interactable.Found = true;
        //         }
        //     }
        }

        private void Do3DRaycast(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << 6);
            
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            Debug.Log("3D-Raycast: //////////////////////////////////////");
            
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].collider.gameObject.name);
            }
            
            foreach (var hit in hits)
            {
                var col = hit.collider;
            
                var interactable = col.gameObject.GetComponentInChildren<Interactable>();
                // var display = col.gameObject.GetComponentInChildren<InteractableDisplay>();
            
                // if(interactable == null)
                //     interactable = col.gameObject.GetComponentInParent<Interactable>();
            
                if(interactable == null)
                    continue;
            
                if(interactable.Found)
                    continue;
            
                interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, interactable.Data);
            
                if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel &&
                    interactable.Data.AwarenessLevel != AwarenessLevel.NotSet)
                {
                    sauerteig.HandleInteractableDiscovered(interactable.Data);
                    interactable.Found = true;
                }
            }
        }
        
        private void Do2DRaycast(Ray ray)
        {
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity, 1 << 6);
            
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            Debug.Log("2D-Raycast: //////////////////////////////////////");
            
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].collider.gameObject.name);
            }
            
            foreach (var hit in hits)
            {
                var col = hit.collider;
            
                var interactable = col.gameObject.GetComponentInChildren<Interactable>();
                // var display = col.gameObject.GetComponentInChildren<InteractableDisplay>();
            
                // if(interactable == null)
                //     interactable = col.gameObject.GetComponentInParent<Interactable>();
            
                if(interactable == null)
                    continue;
            
                if(interactable.Found)
                    continue;
            
                interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, interactable.Data);
            
                // if(display == null)
                //     continue;
            
                if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel)
                {
                    sauerteig.HandleInteractableDiscovered(interactable.Data);
                    interactable.Found = true;
                }
            }
        }
    }
}