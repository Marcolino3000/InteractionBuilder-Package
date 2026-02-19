using System.Collections;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class ToggleTrigger : MonoBehaviour
    {
        [SerializeField] private InteractionsCounter counterToToggle;
        [SerializeField] private bool toggle;
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerController>() == null)
                return;
            
            StartCoroutine(StartPlayerEnteredCooldown());
        }
        
        private IEnumerator StartPlayerEnteredCooldown()
        {
            Toggle();

            yield return new WaitForSeconds(1f);
        }

        private void Toggle()
        {
            toggle = !toggle;
            counterToToggle.SetActive(toggle);
        }
    }
}