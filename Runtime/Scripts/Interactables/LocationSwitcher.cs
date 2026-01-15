using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class LocationSwitcher : MonoBehaviour
    {
        public event Action OnPlayerTrigger;

        [SerializeField] private bool toggle;
        [SerializeField] private List<GameObject> firstLocation;
        [SerializeField] private List<GameObject> secondLocation;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger entered");
            if(other.GetComponent<PlayerController>() == null)
                return;
            
            StartCoroutine(StartPlayerEnteredCooldown());
        }
        
        private IEnumerator StartPlayerEnteredCooldown()
        {
            // OnPlayerTrigger?.Invoke();
            Debug.Log("player triggered location switcher");
            SwitchLocations();

            yield return new WaitForSeconds(1f);
        }

        private void SwitchLocations()
        {
            foreach (var location in firstLocation)
            {
                ToggleExpeptTrigger(location, toggle);
            }
            
            foreach (var location in secondLocation)
            {
                ToggleExpeptTrigger(location, !toggle);
            }
            
            toggle = !toggle;
        }

        private void ToggleGameObjectAndChildren(GameObject obj, bool show)
        {
            if (obj == null) return;
            var renderers = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.enabled = show;
            }
        }
        
        private void ToggleExpeptTrigger(GameObject obj, bool show)
        {
            var allChildren = GetAllChildrenExceptTrigger(obj);
            foreach (var child in allChildren)
            {
                var rend = child.GetComponent<Renderer>();
                if (rend != null)
                    rend.enabled = show;
            }
        }

        private List<GameObject> GetAllChildrenExceptTrigger(GameObject parent)
        {
            var result = new List<GameObject>();
            if (parent == null) 
                return result;
            
            result.Add(parent);
            
            foreach (Transform child in parent.transform)
            {
                
                if (child.name == "Trigger" || child.name == "TriggerArea")
                    continue;
                result.Add(child.gameObject);
                result.AddRange(GetAllChildrenExceptTrigger(child.gameObject));
            }
            return result;
        }
    }
}