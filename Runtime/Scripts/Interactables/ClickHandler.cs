using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Scripts.Interactables
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        public int counter;
        public void OnPointerClick(PointerEventData eventData)
        {
            UnityEngine.Debug.Log("clicked on " + name + " " + counter++);
            Debug.Log("HOVERED");
            foreach (var hov in eventData.hovered) 
            {
                Debug.Log(hov.name);
            }
        }
    }
}