using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Scripts.PlayerInput;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class TransparentWall : MonoBehaviour
    {
        public Action OnPlayerEnter;
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerController>() == null)
                return;
            
            StartCoroutine(StartPlayerEnteredCooldown());
        }

        private IEnumerator StartPlayerEnteredCooldown()
        {
            OnPlayerEnter?.Invoke();
            // Debug.Log("player triggered wall");

            yield return new WaitForSeconds(1f);
        }
    }
}