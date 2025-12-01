using System;
using System.Collections.Generic;
using Runtime.Scripts.Interactables;
using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    public event Action<LocationTrigger> PlayerEntered;
    
    // [SerializeField] private GameObject exterior;
    // [SerializeField] private GameObject interior;
    [SerializeField] private TransparentWall entranceWall;
    [SerializeField] private TransparentWall exitWall;
    [SerializeField] private bool PlayerInLocation;

    private void OnEnable()
    {
        // entranceWall.OnPlayerTrigger += (wall) =>  Toggle(wall, true);
        // exitWall.OnPlayerTrigger += (wall) => Toggle(wall, false);
        if(entranceWall != null)
        {
            entranceWall.OnPlayerTrigger += ToggleEntrance;
        }
        
        if(exitWall != null)
            exitWall.OnPlayerTrigger += ToggleEntrance;
    }

    // private void ToggleExit(TransparentWall wall)
    // {
    //     wall.GetComponent<Renderer>().enabled = PlayerInLocation;
    //     PlayerInLocation = !PlayerInLocation;
    // }

    private void ToggleEntrance(TransparentWall wall)
    {
        wall.GetComponent<Renderer>().enabled = PlayerInLocation;
        PlayerInLocation = !PlayerInLocation;
        
        if(PlayerInLocation)
            PlayerEntered?.Invoke(this);
    }

    private void Toggle(TransparentWall wall, bool wentInside)
    {
        PlayerInLocation = wentInside;
        PlayerEntered?.Invoke(this);

        wall.GetComponent<Renderer>().enabled = !PlayerInLocation;
        // ToggleObjectColliders(PlayerIsInside);
        // exterior.SetActive(!PlayerIsInside);
        // interior.SetActive(PlayerIsInside);
    }

    public void ToggleObjectColliders(bool enable)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(enable);
        }
        // var colliders = GetComponentsInChildren<PolygonCollider2D>();
        // foreach (var col in colliders)
        // {
        //     col.enabled = enable;
        // }
    }
}
