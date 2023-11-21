using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
#if UNITY_IOS
using UnityEngine.XR.ARKit;
#endif


using Google.XR.ARCoreExtensions.Internal;

using UnityEngine.XR.ARSubsystems;

using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using System.Collections.Generic;


public class CustomOverlay : MonoBehaviour
{

    public AREarthManager earthManager;

    public GameObject trackingBox;

    private ARGeospatialCreatorAnchor anchor;

    //private List<GameObject> foundObjects;// = new List<GameObject>();

    //private List<ARGeospatialCreatorObject> foundObjects = new List<ARGeospatialCreatorObject>();

    //var foundObjects;

    public bool supported
    {
        get
        {
#if UNITY_IOS
            return ARKitSessionSubsystem.coachingOverlaySupported;
#else
            return false;
#endif
        }
    }

    void Start(){
        var foundObjects = FindObjectsOfType<ARGeospatialCreatorAnchor>();
        anchor = foundObjects[0];
    }

    void Update(){

        /*
        if(anchor != null && anchor._isResolved == true) /*foundObjects[0]._anchorResolution == AnchorResolutionState.Complete*/
        //if(earthManager.EarthTrackingState == TrackingState.Tracking){
        /*{
            DisableCoaching(true);
            trackingBox.SetActive(true);
            this.gameObject.GetComponent<CustomOverlay>().enabled = false;
        }*/
    }


    public void ActivateCoaching(bool animated)
        {
    #if UNITY_IOS
        if (supported && GetComponent<ARSession>().subsystem is ARKitSessionSubsystem sessionSubsystem)
        {
            sessionSubsystem.SetCoachingActive(true, animated ? ARCoachingOverlayTransition.Animated : ARCoachingOverlayTransition.Instant);
        }
        else
    #endif
        {
            throw new NotSupportedException("ARCoachingOverlay is not supported");
        }
    }

    /// <summary>
    /// Disables the [ARCoachingGoal](https://developer.apple.com/documentation/arkit/arcoachinggoal)
    /// </summary>
    /// <param name="animated">If <c>true</c>, the coaching overlay is animated, e.g. fades out. If <c>false</c>, the coaching overlay disappears instantly, without any transition.</param>
    public void DisableCoaching(bool animated)
    {
    #if UNITY_IOS
        if (supported && GetComponent<ARSession>().subsystem is ARKitSessionSubsystem sessionSubsystem)
        {
            sessionSubsystem.SetCoachingActive(false, animated ? ARCoachingOverlayTransition.Animated : ARCoachingOverlayTransition.Instant);
        }
        else
    #endif
        {
            throw new NotSupportedException("ARCoachingOverlay is not supported");
        }
    }
}
