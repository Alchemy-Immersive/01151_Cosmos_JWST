using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class CeilingFinder : MonoBehaviour
{

    private ARPlaneManager arPlaneManager;
    private List<ARPlane> arPlanes;
    GameObject ceiling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arPlanes = new List<ARPlane>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (ARPlane plane in arPlanes)
        {
            if (plane.classifications == PlaneClassifications.Ceiling && plane.gameObject.activeInHierarchy)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }


    void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> eventArgs)
    {
        if (eventArgs.added != null && eventArgs.added.Count > 0)
        {
            foreach (ARPlane plane in eventArgs.added)
            {
                arPlanes.Add(plane);
            }
        }
        /*
        foreach (var plane in eventArgs.updated)
        {
            if (plane.trackingState == TrackingState.None)
            {
                arPlanes.Remove(plane);
            }
        }*/

        if (eventArgs.removed != null && eventArgs.removed.Count > 0)
        {
            foreach (KeyValuePair<TrackableId, ARPlane> plane in eventArgs.removed)
            {
                arPlanes.Remove(plane.Value);
            }
        }



    }
}
