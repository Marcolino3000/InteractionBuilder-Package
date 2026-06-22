using Runtime.Scripts.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Interactables
{
    public class UiInteractionTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Interactable interactable;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Sauerteig clicked");
            interactable.OnInteractionStarted(InteractionTriggerVia.ButtonPress, interactable.Data);
        }
    }
}