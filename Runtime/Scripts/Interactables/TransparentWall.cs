using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Scripts.Interactables
{
    public class TransparentWall : MonoBehaviour
    {
        public Action OnPlayerEnter;
        private void OnTriggerEnter(Collider other)
        {
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