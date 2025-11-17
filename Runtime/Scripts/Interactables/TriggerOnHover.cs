// using System;
// using System.Collections;
// using UnityEngine;
//
// namespace Runtime.Scripts.Interactables
// {
//     
//     public class TriggerOnHover : MonoBehaviour
//     {
//         [SerializeField] private InteractableDisplay interactableDisplay;
//         [SerializeField] private float cooldownBetweenHovers;
//         [SerializeField] private bool cooldownActive;
//         private void OnMouseEnter()
//         {
//         // {
//         //     if (cooldownActive) 
//         //         return;
//         //     
//         // StartCoroutine(interactableDisplay.QuickFadeInAndOut());
//         // StartCoroutine(StartHoverCooldown());
//         }
//
//         public void TriggerHoverEffect()
//         {
//             if (cooldownActive) 
//                 return;
//             
//             StartCoroutine(StartCooldownCoroutine());
//             StartCoroutine(interactableDisplay.QuickFadeInAndOut());
//         }
//         
//         private IEnumerator StartCooldownCoroutine()
//         {
//             cooldownActive = true;
//             yield return new WaitForSeconds(cooldownBetweenHovers);
//             cooldownActive = false;
//         }
//     }
// }