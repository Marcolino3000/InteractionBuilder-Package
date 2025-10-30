using System;
using Runtime.Scripts.Interactables;
using UnityEngine;

public class InteriorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject exterior;
    [SerializeField] private GameObject interior;
    [SerializeField] private TransparentWall wall;
    [SerializeField] private bool PlayerIsInside;

    private void OnEnable()
    {
        wall.OnPlayerEnter += Toggle;
    }

    private void Toggle()
    {
        PlayerIsInside = !PlayerIsInside;

        wall.GetComponent<Renderer>().enabled = !PlayerIsInside;
        exterior.SetActive(!PlayerIsInside);
        interior.SetActive(PlayerIsInside);
    }
}
