using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class AugmentationController : MonoBehaviour
{
    public SteamVR_Action_Boolean notice;

    public SteamVR_Input_Sources controller;

    public List<GameObject> userInterefaces = new List<GameObject>();

    public GameObject camera;
    
    private float INIT_FRAMES = 300;

    private float augFrames;
    private float curFrames = 0;

    private float timer = 0f;

    public bool scaleAug = false;
    private Vector3 oriScale;
    private Vector3 tarScale;
    private Vector3 minScale;
    private Vector3 maxScale;
    private Vector3 minScale1 = new Vector3(0.10f, 0.10f, 0.10f);
    private Vector3 maxScale1 = new Vector3(0.15f, 0.15f, 0.15f);
    private Vector3 minScale2 = new Vector3(0.4f, 0.2f, 0.001f);
    private Vector3 maxScale2 = new Vector3(0.6f, 0.3f, 0.001f);
    private List<string> iconList = new List<string>();

    private GameObject curObject;

    private string user = "test.txt";
    private StreamWriter writer;

    private bool ifGaze = true;

    string Vector3ToString(Vector3 v)
    {
        string res = v.x + " " + v.y + " " + v.z;
        return res;
    }

    string QuaternionToString(Quaternion q)
    {
        string res = q.x + " " + q.y + " " + q.z + " " + q.w;
        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!ifGaze)
        {
            notice.AddOnStateDownListener(TriggerDown, controller);
        }

        writer = new StreamWriter(user, false);

        augFrames = INIT_FRAMES;
        curObject = userInterefaces[Random.Range(0, userInterefaces.Count)];
        print(curObject.transform.name);

        iconList.Add("Facebook");
        iconList.Add("Ins");
        iconList.Add("Tiktok");
        iconList.Add("Twitter");
        if (iconList.Contains(curObject.transform.name))
        {
            minScale = minScale1;
            maxScale = maxScale1;
        }
        else
        {
            minScale = minScale2;
            maxScale = maxScale2;
        }
        oriScale = minScale;
        tarScale = maxScale;
        // foreach (GameObject obj in userInterefaces)
        // {
        //     float x = Random.Range(-0.5f, 0.5f);
        //     float y = Random.Range(0.5f, 1.5f);
        //     float z = Random.Range(0.5f, 1.5f);
        //     obj.transform.position = new Vector3(x, y, z);
        // }
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        curObject.transform.localScale = minScale;
        scaleAug = false;
        augFrames = INIT_FRAMES;
        curFrames = 0;
        curObject = userInterefaces[Random.Range(0, userInterefaces.Count)];
        print(curObject.transform.name);
        writer.WriteLine("Noticed" + " " + Time.time);
        writer.Flush();
        if (iconList.Contains(curObject.transform.name))
        {
            minScale = minScale1;
            maxScale = maxScale1;
        }
        else
        {
            minScale = minScale2;
            maxScale = maxScale2;
        }
        oriScale = minScale;
        tarScale = maxScale;
    }

    void OnApplicationQuit()
    {
        writer.Flush();
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.A))
        {
            scaleAug = true;
            writer.WriteLine(curObject.transform.name + " " + Time.time);
            writer.Flush();
        }        
        if (Input.GetKeyDown(KeyCode.S))
        {
            scaleAug = false;
        }
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (scaleAug)
        {   
            // ScreenCapture.CaptureScreenshot("./Screenshots/screenshot" + Time.time + ".png");
            // int camLayerMask = 1 << 3;
            // camera.GetComponent<Camera>().cullingMask = camLayerMask;
            // ScreenCapture.CaptureScreenshot("./Gaze/gaze" + Time.time + ".png");
            // camLayerMask = 1 << 7;
            // camera.GetComponent<Camera>().cullingMask = ~camLayerMask;
            string t = "Camera: " + Vector3ToString(camera.transform.position) + " " + QuaternionToString(camera.transform.rotation) + " " + Time.time;
            writer.WriteLine(t);
            writer.Flush();
            if(ifGaze && eyeTrackingData.GazeRay.IsValid)
            {   
                string g = "Gaze: " + Vector3ToString(eyeTrackingData.GazeRay.Origin) + " " + Vector3ToString(eyeTrackingData.GazeRay.Direction) + " " + Time.time;
                writer.WriteLine(g);
                writer.Flush();
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
                    writer.WriteLine("Noticed" + " " + Time.time);
                    writer.Flush();
                    if (iconList.Contains(curObject.transform.name))
                    {
                        minScale = minScale1;
                        maxScale = maxScale1;
                    }
                    else
                    {
                        minScale = minScale2;
                        maxScale = maxScale2;
                    }
                    oriScale = minScale;
                    tarScale = maxScale;
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
