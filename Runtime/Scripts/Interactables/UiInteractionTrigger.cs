using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Interactables
{
    public class UiInteractionTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private InteractableState interactable;


        public void OnPointerDown(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("pointer down");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("click");
        }
    }
}