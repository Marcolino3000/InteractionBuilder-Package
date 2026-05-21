using System;
using System.Collections.Generic;
using Runtime.Scripts.Core;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Raycaster : MonoBehaviour
    {
        public event Action OnCancelShowInteractable;
        public event Action OnClick;
        
        public bool isDialogRunning;
        public bool isShowingInteractable;
        
        [Header("Settings")]
        [SerializeField] private bool logHits;
        [SerializeField] private bool disableMovementDuringDialog;

        [SerializeField] private DialogTreeRunner dialogTreeRunner;
        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask interactablesLayerMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private MoveByClick moveByClick;
        [SerializeField] private CursorSetter cursorSetter;
        [SerializeField] private InteractableDisplay currentlyHoveredInteractable;
        [SerializeField] private InteractableDisplay lastHoveredInteractable;
        
        private Camera cam;
        private bool clickedWall;
        private bool stoppedHoveringInteractableLastFrame;

        private void OnEnable()
        {
            cam = Camera.main;
            
            // cursorSetter.Initialize();
            
            DialogTreeRunner.OnDialogRunningStatusChanged += (isRunning, tree) =>
            {
                isDialogRunning = isRunning;

                if(isRunning)
                    cursorSetter.SetStandardCursor();
            };

            SequenceRunner.OnSequenceRunningChanged += (isRunning) =>
            {
                isDialogRunning = isRunning;

                if(isRunning)
                    cursorSetter.SetStandardCursor();
            };
        }

        void Update()
        {
            if (disableMovementDuringDialog && isDialogRunning)
                return; 

            HandleHoverOnInteractables();
        }

        public void HandleMouseClick()
        {
            if(disableMovementDuringDialog && isDialogRunning)
                return;

            if (isShowingInteractable)
            {
                isShowingInteractable = false;
                OnCancelShowInteractable?.Invoke();
                return;
            }

            HandleClickOnInteractables();
        }

        private void HandleHoverOnInteractables()
        {
            var hits = GetAllSortedRaycastHits();
            bool hoveringInteractable = false;
            Interactable hoveredInteractable = null;
            var interactionType = InteractionType.None;
            
            foreach (var hit in hits)
            {
                if (hit.target.layer == LayerMask.NameToLayer("Walls"))
                    break;

                if (hit.target.layer == LayerMask.NameToLayer("Marlene"))
                    break;

                if (hit.target.layer == LayerMask.NameToLayer("Scene Plane"))
                {
                    interactionType = InteractionType.Move;
                }
                
                if (isDialogRunning)
                    continue; 

                if (hit.interactable == null)
                {
                    continue;
                }

                if (hit.target.layer == LayerMask.NameToLayer("Walls"))
                    return;

                if (hit.isTrigger)
                    continue;

                if(sauerteig.awarenessLevel < hit.interactable.Data.AwarenessLevel)
                    continue;

                hoveringInteractable = true;
                stoppedHoveringInteractableLastFrame = true;
                interactionType = hit.interactable.Data.InteractionType;
                hoveredInteractable = hit.interactable;

                if(sauerteig == null)
                    Debug.LogWarning("sauerteig is null");


                if (hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                    ShowStandardOutline(hit);
                
                if (!sauerteig.IsUnlocked)
                    continue;

                if (hit.interactable.Data.AwarenessLevel >= AwarenessLevel.Basic)
                    ShowSpecialOutline(hit);


                // hit.display?.TriggerHoverEffect();
                
                
                // sauerteig.GetGlowManager().Glow();
                hoveredInteractable = hit.interactable;

                break;
            }
            
            if(!hoveringInteractable)
            {
                if(stoppedHoveringInteractableLastFrame)
                {
                    stoppedHoveringInteractableLastFrame = false;
                    currentlyHoveredInteractable?.HideStandardOutline();
                    currentlyHoveredInteractable?.HideStaticSpecialOutline();
                    sauerteig.GetGlowManager().HideStaticGlow();

                    currentlyHoveredInteractable = null;
                }
            }
            
            cursorSetter.SetCursor(interactionType);

            // if(hoveredInteractable != null)
            //     cursorSetter.SetCursor(hoveredInteractable.Data.InteractionType);
        }

        private void ShowStandardOutline(RaycastInteractableHit hit)
        {
            if (currentlyHoveredInteractable == hit.display) 
                return;
            
            currentlyHoveredInteractable?.HideStandardOutline();
            
            currentlyHoveredInteractable = hit.display;
            
            currentlyHoveredInteractable?.ShowStandardOutline();
        }

        private void ShowSpecialOutline(RaycastInteractableHit hit)
        {
            if (currentlyHoveredInteractable == hit.display) 
                return;
            
            currentlyHoveredInteractable?.HideStaticSpecialOutline();
            sauerteig.GetGlowManager().HideStaticGlow();
            
            currentlyHoveredInteractable = hit.display;
            
            currentlyHoveredInteractable?.ShowStaticSpecialOutline();
            
            if(currentlyHoveredInteractable != null)
                sauerteig.GetGlowManager().ShowStaticGlow();
        }

        private void HandleClickOnInteractables()
        {
            var hits = GetAllSortedRaycastHits();

            if (logHits)
            {
                Debug.Log("//////HITS///////////////////");

                foreach (var hit in hits)
                {
                    Debug.Log(hit.target.name + ": " + hit.distance);
                }
            }

            OnClick?.Invoke();
            
            foreach (var hit in hits)
            {
                if (hit.target.layer == LayerMask.NameToLayer("Walls"))
                    return;

                if (hit.target.layer == LayerMask.NameToLayer("Marlene"))
                    return;

                if(hit.target.name == "Wall")
                    continue;

                if (hit.target.layer == LayerMask.NameToLayer("Scene Plane"))
                {
                    moveByClick.HandleMouseClick();
                    return;
                }     
                
                if(isDialogRunning)
                    continue;

                if (hit.interactable == null)
                    continue;

                if (hit.interactable.Found)
                    continue;

                if(hit.isTrigger)
                    continue;
                
                Debug.Log("Raycaster: Clicked on interactable: " + hit.interactable.Data.name + ", AwarenessLevel: " + hit.interactable.Data.AwarenessLevel);

                if(hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                    hit.interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, hit.interactable.Data);

                if (hit.interactable.Data.AwarenessLevel != AwarenessLevel.NotSet && !sauerteig.IsUnlocked)
                {
                    hit.interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, hit.interactable.Data);
                }

                else if(hit.interactable.Data.AwarenessLevel != AwarenessLevel.NotSet && sauerteig.awarenessLevel >= hit.interactable.Data.AwarenessLevel)
                {
                    hit.interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, hit.interactable.Data);

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
            public bool isTrigger;
            public RaycastInteractableHit(Interactable interactable, InteractableDisplay display, float distance, GameObject target, bool isTrigger = false)
            {
                this.interactable = interactable;
                this.display = display;
                this.distance = distance;
                this.target = target;
                this.isTrigger = isTrigger;
            }
        }

        private bool GetHitsFrom3DRaycast(List<Raycaster.RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            var layerMask = wallLayerMask | interactablesLayerMask | groundLayerMask;
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                interactables.Add(new Raycaster.RaycastInteractableHit(interactable, display, hit.distance, hit.collider.gameObject));
            }
            return interactables.Count > 0;
        }

        private bool GetHitsFrom2DRaycast(List<Raycaster.RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            var hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                interactables.Add(new Raycaster.RaycastInteractableHit(interactable, display, hit.distance, hitCollider.gameObject, NameContainsTrigger(interactable?.gameObject)));
            }
            return interactables.Count > 0;
        }

        private bool NameContainsTrigger(GameObject interactableGameObject)
        {
            return interactableGameObject != null && interactableGameObject.name.ToLower().Contains("trigger");
        }

        private List<Raycaster.RaycastInteractableHit> GetAllSortedRaycastHits()
        {
            var interactables3D = new List<Raycaster.RaycastInteractableHit>();
            var interactables2D = new List<Raycaster.RaycastInteractableHit>();
            GetHitsFrom3DRaycast(interactables3D);
            GetHitsFrom2DRaycast(interactables2D);
            var allInteractables = new List<Raycaster.RaycastInteractableHit>(interactables3D);
            allInteractables.AddRange(interactables2D);
            allInteractables.Sort((a, b) => a.distance.CompareTo(b.distance));
            return allInteractables;
        }
    }
}