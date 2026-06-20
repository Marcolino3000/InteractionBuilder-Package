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
        public event Action<Interactable> OnInteractableClicked;
        public event Action<Vector3> OnGroundClicked; 
        
        [Header("Debug")]
        public bool isDialogRunning;
        public bool isSequenceRunning;
        public bool isShowingInteractable;
        [SerializeField] private InteractableDisplay currentlyHoveredInteractable;
        [SerializeField] private InteractableDisplay lastHoveredInteractable;

        [Header("Settings")]
        [SerializeField] private bool logClickHits;
        [SerializeField] private bool logHoverHits;
        [SerializeField] private bool disableMouseInputDuringDialog;
        [SerializeField] private bool disableMouseInputDuringSequences;

        [Header("References")]
        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask interactablesLayerMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private MoveByClick moveByClick;
        [SerializeField] private CursorSetter cursorSetter;

        private Camera cam;
        private bool clickedWall;
        private bool stoppedHoveringInteractableLastFrame;
        private int wallLayer;
        private int groundLayer;
        private int playerLayer;

        private void OnEnable()
        {
            wallLayer = LayerMask.NameToLayer("Walls");
            groundLayer = LayerMask.NameToLayer("Scene Plane");
            playerLayer = LayerMask.NameToLayer("Marlene");
            
            cam = Camera.main;
            
            DialogTreeRunner.OnDialogRunningStatusChanged += (isRunning, tree) =>
            {
                isDialogRunning = isRunning;

                if(isRunning)
                    cursorSetter.SetStandardCursor();
            };

            SequenceRunner.OnSequenceRunningChanged += (isRunning) =>
            {
                isSequenceRunning = isRunning;

                if(isRunning)
                    cursorSetter.SetStandardCursor();
            };
        }

        void Update()
        {
            if (disableMouseInputDuringDialog && isDialogRunning)
                return; 
            
            if(disableMouseInputDuringSequences && isSequenceRunning)
                return;

            HandleHoverOnInteractables();
        }

        public void HandleMouseClick()
        {
            if(disableMouseInputDuringDialog && isDialogRunning)
                return;
            
            if(disableMouseInputDuringSequences && isSequenceRunning)
                return;

            if (isShowingInteractable)
            {
                isShowingInteractable = false;
                OnCancelShowInteractable?.Invoke();
                return;
            }

            HandleClickOnInteractables();
        }

        private void HandleClickOnInteractables()
        {
            var hits = GetAllSortedRaycastHits();

            if (logClickHits)
            {
                Debug.Log("//////HITS///////////////////");

                foreach (var hit in hits)
                {
                    Debug.Log(hit.target.name + ": " + hit.distance);
                }
            }
            
            foreach (var hit in hits)
            {
                if (hit.target.layer == wallLayer)
                    return;

                if (hit.target.layer == LayerMask.NameToLayer("Marlene"))
                    return;

                if(hit.target.name == "Wall")
                    continue;

                if (hit.target.layer == LayerMask.NameToLayer("Scene Plane"))
                {
                    // moveByClick.HandleMouseClick();
                    // OnInteractableClicked?.Invoke(false, null);
                    var targetPosition = GetTargetPosition();
                    OnGroundClicked?.Invoke(targetPosition);
                    return;
                }     
                
                // if(isDialogRunning)
                //     continue;

                if (hit.interactable == null)
                    continue;

                if (hit.interactable.Found)
                    continue;

                if(hit.isTrigger)
                    continue;
                
                Debug.Log("Raycaster: Clicked on interactable: " + hit.interactable.Data.name + ", AwarenessLevel: " + hit.interactable.Data.AwarenessLevel);

                OnInteractableClicked?.Invoke(hit.interactable);

                //only process the interactable closest to camera
                return;
            }
        }

        private Vector3 GetTargetPosition()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                return hit.point;
            }

            Debug.LogError("Raycast did not hit the ground layer. Returning zero vector.");
            
            return Vector3.zero;
        }

        private void HandleHoverOnInteractables()
        {
            var hits = GetAllSortedRaycastHits();
            bool hoveringInteractable = false;
            RaycastInteractableHit hoveredInteractable = default;
            
            var hoverStatus = CheckHoverStatus(hits, ref hoveredInteractable);

            if(logHoverHits)
            {
                Debug.Log("HITS////////////////////////");
                foreach (var hit in hits)
                {

                    Debug.Log(hit.target.name);
                }
            }

            switch (hoverStatus)
            {
                case HoverStatus.None:
                    cursorSetter.SetCursor(InteractionType.None);
                    hoveringInteractable = false;
                    break;
                case HoverStatus.Ground:
                    cursorSetter.SetCursor(InteractionType.Move);
                    hoveringInteractable = false;
                    break;
                case HoverStatus.BasicInteractable:
                    ShowStandardOutline(hoveredInteractable);
                    hoveringInteractable = true;
                    stoppedHoveringInteractableLastFrame = true;
                    cursorSetter.SetCursor(hoveredInteractable.interactable.Data.InteractionType);
                    break;
                case HoverStatus.SpecialInteractable:
                    if (!sauerteig.IsUnlocked)
                        break;
                    ShowSpecialOutline(hoveredInteractable);
                    hoveringInteractable = true;
                    stoppedHoveringInteractableLastFrame = true;
                    cursorSetter.SetCursor(hoveredInteractable.interactable.Data.InteractionType);
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
        }

        private HoverStatus CheckHoverStatus(List<RaycastInteractableHit> hits, ref RaycastInteractableHit hoveredInteractable)
        {
            // InteractionType interactionType;
            foreach (var hit in hits)
            {
                if (hit.target.layer == wallLayer || hit.target.layer == playerLayer)
                {
                    return HoverStatus.None;
                }

                if (hit.target.layer == groundLayer)
                {
                    return HoverStatus.Ground;
                }

                if (hit.isTrigger)
                    continue;

                if (hit.interactable == null)
                    continue;
                
                if (sauerteig.awarenessLevel < hit.interactable.Data.AwarenessLevel)
                {
                    cursorSetter.SetCursor(InteractionType.None);
                    return HoverStatus.None;
                }

                hoveredInteractable  = hit;
                
                switch (hit.interactable.Data.AwarenessLevel)
                {
                    case AwarenessLevel.NotSet:
                        // ShowStandardOutline(hit);
                        return HoverStatus.BasicInteractable;
                        break;
                    case > AwarenessLevel.NotSet:
                        // if (!sauerteig.IsUnlocked)
                            // return true;
                        // ShowSpecialOutline(hit);
                        return HoverStatus.SpecialInteractable;
                        break;
                }
                
                // hoveringInteractable = true;
                // stoppedHoveringInteractableLastFrame = true;
                // interactionType = hit.interactable.Data.InteractionType;
                
                
                
                // if (hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                //     ShowStandardOutline(hit);
                // if (hit.interactable.Data.AwarenessLevel > AwarenessLevel.NotSet)
                //     ShowSpecialOutline(hit);

                break;
            }

            return HoverStatus.None;
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

        private bool GetHitsFrom3DRaycast(List<RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            var layerMask = wallLayerMask | interactablesLayerMask | groundLayerMask;
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
            foreach (var hit in hits)
            {
                var hitCollider = hit.collider;
                var interactable = hitCollider.gameObject.GetComponentInChildren<Interactable>();
                var display = hitCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                
                // if(interactable == null)
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
                
                // if(interactable == null)
                //     continue;
                
                interactables.Add(new RaycastInteractableHit(interactable, display, hit.distance, hitCollider.gameObject, NameContainsTrigger(interactable?.gameObject)));
            }
            return interactables.Count > 0;
        }

        private bool NameContainsTrigger(GameObject interactableGameObject)
        {
            return interactableGameObject != null && interactableGameObject.name.ToLower().Contains("trigger");
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

    enum HoverStatus
    {
        None,
        Ground,
        BasicInteractable,
        SpecialInteractable
    }
}