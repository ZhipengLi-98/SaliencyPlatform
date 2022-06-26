using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;

public class GetGaze : MonoBehaviour
{
    public GameObject gazeCursor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        // Get eye tracking data in world space
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);

        // Check if gaze ray is valid
        if(eyeTrackingData.GazeRay.IsValid)
        {
            // The origin of the gaze ray is a 3D point
            var rayOrigin = eyeTrackingData.GazeRay.Origin;
            print(rayOrigin);

            // The direction of the gaze ray is a normalized direction vector
            var rayDirection = eyeTrackingData.GazeRay.Direction;

            gazeCursor.transform.position = rayOrigin + 1.0f * rayDirection;
        }
    }
}
