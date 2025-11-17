using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Raycaster : MonoBehaviour
    {
        private Camera cam;
        private Sauerteig sauerteig;
        // [SerializeField] private float cooldownBetweenHovers;

        private void OnEnable()
        {
            cam = Camera.main;
            sauerteig = GetComponent<Sauerteig>();
        }

        void Update()
        {
            DoRaycast();
        }

        private void DoRaycast()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (var hit in hits)
            {
                var col = hit.collider;
                if (col is MeshCollider meshCollider)
                {
                    var display = meshCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
                    // display?.TriggerHoverEffect();

                    var interactable = meshCollider.gameObject.GetComponentInChildren<Interactable>();

                    if(interactable == null)
                        continue;
                    
                    if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel)
                    {
                        display.TriggerHoverEffect();
                    }
                    
                }
            }
            
            // var hits2D = Physics2D.RaycastAll(ray.origin, ray.direction);
            // foreach (var hit in hits2D)
            // {
            //     var col = hit.collider;
            //     if (col is PolygonCollider2D polygonCollider)
            //     {
            //         var display = polygonCollider.gameObject.GetComponentInChildren<InteractableDisplay>();
            //         display?.TriggerHoverEffect();
            //     }
            // }
        }

        public void HandleMouseClick()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);

            foreach (var hit in hits)
            {
                var col = hit.collider;
                
                var interactable = col.gameObject.GetComponentInChildren<Interactable>();
                var display = col.gameObject.GetComponentInChildren<InteractableDisplay>();
                
                if(interactable == null)
                    continue;
                
                if(display == null)
                    continue;
                
                if (sauerteig.awarenessLevel >= interactable.Data.AwarenessLevel)
                {
                    sauerteig.HandleInteractableDiscovered(interactable.Data);
                }
            }
        }
    }
}