using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;

public class AugmentationController : MonoBehaviour
{
    public List<GameObject> userInterefaces = new List<GameObject>();
    
    private float INIT_FRAMES = 300;

    private float augFrames;
    private float curFrames = 0;

    private float timer = 0f;

    public bool scaleAug = false;
    private Vector3 oriScale;
    private Vector3 tarScale;
    private Vector3 minScale = new Vector3(0.10f, 0.10f, 0.10f);
    private Vector3 maxScale = new Vector3(0.15f, 0.15f, 0.15f);

    private GameObject curObject;

    // Start is called before the first frame update
    void Start()
    {
        augFrames = INIT_FRAMES;
        oriScale = minScale;
        tarScale = maxScale;
        curObject = userInterefaces[Random.Range(0, userInterefaces.Count)];
        print(curObject.transform.name);
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.A))
        {
            scaleAug = true;
        }
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (scaleAug)
        {
            if(eyeTrackingData.GazeRay.IsValid)
            {   
                int layerMask = 1 << 6;
                RaycastHit hit;
                if (Physics.Raycast(eyeTrackingData.GazeRay.Origin, eyeTrackingData.GazeRay.Direction, out hit, Mathf.Infinity, layerMask))
                {
                    if (Equals(hit.transform, this.transform))
                    {
                        timer += Time.deltaTime;
                    }
                    else
                    {
                        timer = 0f;
                    }
                }
                if (timer >= 1.5f)
                {
                    curObject.transform.localScale = minScale;
                    scaleAug = false;
                    augFrames = INIT_FRAMES;
                    curFrames = 0;
                    curObject = userInterefaces[Random.Range(0, userInterefaces.Count)];
                    print(curObject.transform.name);
                }
            }

            float interpolationRatio = (float) curFrames / augFrames;
            Vector3 interpolatedScale = Vector3.Lerp(oriScale, tarScale, interpolationRatio);
            curObject.transform.localScale = interpolatedScale;
            if (interpolationRatio == 1)
            {
                if (tarScale.x < oriScale.x)
                {
                    augFrames *= 0.8f;
                }
                // Vector3 temp = new Vector3(oriScale.x, oriScale.y, oriScale.z);
                Vector3 temp = oriScale;
                oriScale = tarScale;
                tarScale = temp;
            }
            curFrames = (curFrames + 1) % (augFrames + 1);
        }
    }
}
