using System.Collections.Generic;
using Runtime.Scripts.Core;
using Runtime.Scripts.PlayerInput;
using Tree;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Raycaster : MonoBehaviour
    {
        
        [SerializeField] private bool logHits;
        [SerializeField] private Texture2D standardCursor;
        [SerializeField] private Texture2D hoverInteractableCursor;
        [SerializeField] private int standardCursorSize = 24;
        [SerializeField] private int interactionCursorSize = 32;
        
        [SerializeField] private DialogTreeRunner dialogTreeRunner;
        [SerializeField] private Sauerteig sauerteig;
        [SerializeField] private bool isDialogRunning;
        [SerializeField] private TransparentWall transparentWall;
        [SerializeField] private bool playerIsInside;
        [SerializeField] private LayerMask wallLayerMask;
        [SerializeField] private LayerMask interactablesLayerMask;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private MoveByClick moveByClick;
        
        private Camera cam;
        private bool clickedWall;
        private Texture2D resizedStandardCursor;
        private Texture2D resizedHoverCursor;

        private Texture2D ResizeCursor(Texture2D original, int size)
        {
            if (original == null) return null;
            Texture2D resized = new Texture2D(size, size, original.format, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = x / (float)size;
                    float v = y / (float)size;
                    resized.SetPixel(x, y, original.GetPixelBilinear(u, v));
                }
            }
            resized.Apply();
            return resized;
        }

        private void OnEnable()
        {
            cam = Camera.main;
            resizedStandardCursor = ResizeCursor(standardCursor, standardCursorSize);
            resizedHoverCursor = ResizeCursor(hoverInteractableCursor, interactionCursorSize);
            Cursor.SetCursor(resizedStandardCursor, Vector2.zero, CursorMode.Auto);
            
            DialogTreeRunner.OnDialogRunningStatusChanged += (status, tree) =>
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
            bool hoveringInteractable = false;
            foreach (var hit in hits)
            {
                if (hit.interactable == null)
                {
                    continue;
                }
                
                hoveringInteractable = true;
                
                if(sauerteig == null)
                    Debug.LogWarning("sauerteig is null");
                
                if (!sauerteig.IsUnlocked)
                    continue;

                if (hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                    continue;

                if(sauerteig.awarenessLevel < hit.interactable.Data.AwarenessLevel)
                    continue;

                hit.display?.TriggerHoverEffect();
                sauerteig.GetGlowManager().Glow();
                
                break;
            }

            Debug.Log("hovering interactable: " + hoveringInteractable);
            
            Cursor.SetCursor(hoveringInteractable ? resizedHoverCursor : resizedStandardCursor, Vector2.zero,
                CursorMode.Auto);
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

            // if (!playerIsInside && (hits.Count == 0 || hits[0].target == null || hits[0].target.name == "Wall"))
            // {
            //     // Debug.Log("click raycast early return");
            //     return;
            // }

            foreach (var hit in hits)
            {
                if (hit.target.layer == LayerMask.NameToLayer("Walls"))
                    return;
                    
                if(hit.target.name == "Wall")
                    continue;

                if (hit.target.layer == LayerMask.NameToLayer("Scene Plane"))
                {
                    moveByClick.HandleMouseClick();
                    return;
                }            
                
                if (hit.interactable == null)
                    continue;
                
                if (hit.interactable.Found)
                    continue;
                
                if(hit.isTrigger)
                    continue;
                
                if(hit.interactable.Data.AwarenessLevel == AwarenessLevel.NotSet)
                    hit.interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, hit.interactable.Data);
                
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

        private bool GetHitsFrom3DRaycast(List<RaycastInteractableHit> interactables)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // var layerMask = LayerMask.GetMask("Interactables", "Wall");
            var layerMask = wallLayerMask | interactablesLayerMask | groundLayerMask;
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);
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
                interactables.Add(new RaycastInteractableHit(interactable, display, hit.distance, hitCollider.gameObject, NameContainsTrigger(interactable?.gameObject)));
            }
            return interactables.Count > 0;
        }

        private bool NameContainsTrigger(GameObject interactableGameObject)
        {
            return interactableGameObject.name.ToLower().Contains("trigger");
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