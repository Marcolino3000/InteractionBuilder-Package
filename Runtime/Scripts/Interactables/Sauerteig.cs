using System.Collections;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class Sauerteig : MonoBehaviour
    {
        public AwarenessLevel awarenessLevel;
        public int Activity = 1;
        
        // [SerializeField] private float radarRadius;
        
        private SphereCollider radarCollider;
        
         
        private void Awake()
        {
            radarCollider = GetComponent<SphereCollider>();
        }


        public void ActivateRadar()
        {
            radarCollider.enabled = true;
            
            StartCoroutine(DeactivateRadarNextFrame());
        }

        private IEnumerator DeactivateRadarNextFrame()
        {
            yield return new WaitForSeconds(0.1f);
            
            radarCollider.enabled = false;
            
            if(awarenessLevel != AwarenessLevel.Basic)
                SetActivity(-1);
        }

        public void HandleInteractableDiscovered(InteractableState interactable)
        {
            switch (interactable.AwarenessLevel)
            {
                case AwarenessLevel.Basic:
                    SetActivity(1);
                    break;
                case AwarenessLevel.Super:
                    SetActivity(2);
                    break;
            }
        }

        private void SetActivity(int activityChange)
        {
            Activity += activityChange;
            
            if(Activity < 1)
                Activity = 1;
            
            switch (Activity)
            {
                case 0 or 1:   
                    awarenessLevel = AwarenessLevel.Basic;
                    break;
                case >= 2:
                    awarenessLevel = AwarenessLevel.Super;
                    break;
            }
        }
    }
    
    public enum AwarenessLevel
    {
        NotSet,
        Basic,
        Super
    }
}