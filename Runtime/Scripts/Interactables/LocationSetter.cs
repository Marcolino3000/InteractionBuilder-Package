using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class LocationSetter : MonoBehaviour
    {
        [SerializeField] private List<LocationTrigger> locationTriggers;
        private void OnEnable()
        {
            locationTriggers = GetComponentsInChildren<LocationTrigger>().ToList();
            foreach (var trigger in locationTriggers)
            {
                trigger.PlayerEntered += OnPlayerEntered;
            }
        }

        private void OnPlayerEntered(LocationTrigger trigger)
        {
            foreach (var location in locationTriggers)
            {
                location.ToggleObjectColliders(false);
            }
            
            trigger.ToggleObjectColliders(true);
        }
    }
}