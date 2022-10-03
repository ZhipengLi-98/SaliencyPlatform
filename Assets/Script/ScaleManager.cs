using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System;

public class ScaleManager : MonoBehaviour
{
    public GameObject VirtualHome;
    public GameObject VirtualLab;
    public GameObject VirtualCafe;
    public GameObject PhysicalHome1;
    public GameObject PhysicalHome2;
    public GameObject PhysicalHome3;

    public enum Background {
        VirtualHome, VirtualLab, VirtualCafe, PhysicalHome1, PhysicalHome2, PhysicalHome3
    }
    public Background curBackground;

    public SteamVR_Action_Boolean notice;

    public SteamVR_Input_Sources controller;

    public List<GameObject> userInterefaces = new List<GameObject>();

    public GameObject camera;

    [Range(1, 3)]
    public int startLevel = 1;

    private int INIT_FRAMES = 600;

    private int augFrames;
    private int curFrames = 0;

    private float timer = 0f;

    public bool scaleAug = false;
    private Vector3 oriScale;
    private Vector3 tarScale;
    private Vector3 minScale;
    private Vector3 maxScale;
    public List<GameObject> iconList = new List<GameObject>();
    public List<GameObject> viewerList = new List<GameObject>();
    public GameObject videoPlayer;
    public GameObject keyboard;

    private GameObject curObject;

    public string user = "test.txt";
    private StreamWriter writer;

    public bool ifGaze = false;

    private int augLayer;
    private int norLayer;

    public Material redMaterial;
    private Material oriMaterial;
    private GameObject redObject;

    private string layoutFile = "./layout.txt";
    private List<Dictionary<string, List<Vector3>>> layout;
    private int layoutCnt = 0;

    private bool isAug = false;
    private float augTimer = 0f;

    private bool isWait = false;
    private float waitTimer = 0f;

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
    
    void ReadLayout()
    {
        StreamReader reader = new StreamReader(layoutFile);
        string[] content = reader.ReadToEnd().Split(new string[] { "Start" }, StringSplitOptions.None);
        for (int i = 1; i < content.Length; i++)
        {
            string[] line = content[i].Split('\n');
            Dictionary<string, List<Vector3>> cur = new Dictionary<string, List<Vector3>>();
            for (int j = 1; j < line.Length - 1; j++)
            {
                string[] temp = line[j].Split(new string[] { ", " }, StringSplitOptions.None);
                if (!cur.ContainsKey(temp[0]))
                {
                    List<Vector3> n = new List<Vector3>();
                    cur.Add(temp[0], n);
                }
                cur[temp[0]].Add(new Vector3(float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3])));
            }
            layout.Add(cur);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (startLevel == 1)
        {
            INIT_FRAMES = 600;
        }
        else if (startLevel == 2)
        {
            INIT_FRAMES = 450;
        }
        else if (startLevel == 3)
        {
            INIT_FRAMES = 300;
        }
        VirtualHome.SetActive(false);
        VirtualLab.SetActive(false);
        VirtualCafe.SetActive(false);
        PhysicalHome1.SetActive(false);
        PhysicalHome2.SetActive(false);
        PhysicalHome3.SetActive(false);
        switch (curBackground)
        {
            case (Background.VirtualHome):
                VirtualHome.SetActive(true);
                break;
            case (Background.VirtualLab):
                VirtualLab.SetActive(true);
                break;
            case (Background.VirtualCafe):
                VirtualCafe.SetActive(true);
                break;
            case (Background.PhysicalHome1):
                PhysicalHome1.SetActive(true);
                break;
            case (Background.PhysicalHome2):
                PhysicalHome2.SetActive(true);
                break;
            case (Background.PhysicalHome3):
                PhysicalHome3.SetActive(true);
                break;
            default:
                break;
        }
        
        augLayer = LayerMask.NameToLayer("AugObj");
        norLayer = LayerMask.NameToLayer("NorObj");
        if (!ifGaze)
        {
            notice.AddOnStateUpListener(TriggerUp, controller);
            notice.AddOnStateDownListener(TriggerDown, controller);
        }

        writer = new StreamWriter(user, false);

        augFrames = INIT_FRAMES;

        layout = new List<Dictionary<string, List<Vector3>>>();
        ReadLayout();
        NextLayout();
    }

    private void NextLayout()
    {
        List<int> list = new List<int>();
        for (int n = 0; n < iconList.Count; n++) 
        {
            list.Add(n);
        }
        foreach (GameObject icon in iconList)
        {
            int index = UnityEngine.Random.Range(0, list.Count - 1);
            int i = list[index];
            list.RemoveAt(index);
            icon.transform.position = layout[layoutCnt]["Icon"][i];
            icon.transform.LookAt(camera.transform);
            if (icon.transform.name == "HMDModel")
            {
                icon.transform.rotation = icon.transform.rotation * Quaternion.Euler(0, 180, 0);
            }
            icon.GetComponent<Renderer>().material.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1.0f, 1.0f);
        }
        list.Clear();
        for (int n = 0; n < viewerList.Count; n++) 
        {
            list.Add(n);
        }
        foreach (GameObject viewer in viewerList)
        {
            int index = UnityEngine.Random.Range(0, list.Count - 1);
            int i = list[index];
            list.RemoveAt(index);
            viewer.transform.position = layout[layoutCnt]["Viewer"][i];
            viewer.transform.LookAt(camera.transform);
            if (viewer.transform.name == "TimeWidget")
            {
                viewer.transform.rotation = viewer.transform.rotation * Quaternion.Euler(0, 180, 0);
            }
            viewer.GetComponent<Renderer>().material.color = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1.0f, 1.0f);
        }
        keyboard.transform.position = layout[layoutCnt]["Keyboard"][0];
        keyboard.transform.LookAt(camera.transform);
        keyboard.transform.rotation = keyboard.transform.rotation * Quaternion.Euler(0, 180, 0);
        videoPlayer.transform.position = layout[layoutCnt]["VideoPlayer"][0];
        videoPlayer.transform.LookAt(camera.transform);
        int next = UnityEngine.Random.Range(0, layout.Count);
        while (next == layoutCnt)
        {
            next = UnityEngine.Random.Range(0, layout.Count);
        }
        layoutCnt = next;
        
        if (curObject != null)
        {
            curObject.transform.localScale = minScale;
            curObject.layer = norLayer;
            curObject.GetComponent<Renderer>().material = oriMaterial;
        }
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        minScale = new Vector3(curObject.transform.localScale.x, curObject.transform.localScale.y, curObject.transform.localScale.z);
        maxScale = new Vector3(1.5f * curObject.transform.localScale.x, 1.5f * curObject.transform.localScale.y, 1.5f * curObject.transform.localScale.z);
        oriScale = minScale;
        tarScale = maxScale;
        oriMaterial = curObject.GetComponent<Renderer>().material;
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        curObject.transform.localScale = minScale;
        curObject.layer = norLayer;
        curObject.GetComponent<Renderer>().material = oriMaterial;
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        curObject.layer = augLayer;
        print(curObject.transform.name);
        writer.WriteLine("Noticed" + " " + Time.time);
        writer.Flush();
        
        minScale = new Vector3(curObject.transform.localScale.x, curObject.transform.localScale.y, curObject.transform.localScale.z);
        maxScale = new Vector3(1.5f * curObject.transform.localScale.x, 1.5f * curObject.transform.localScale.y, 1.5f * curObject.transform.localScale.z);

        oriScale = minScale;
        tarScale = maxScale;
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        oriMaterial = curObject.GetComponent<Renderer>().material;
        print(oriMaterial.name);
        curObject.GetComponent<Renderer>().material = redMaterial;
        scaleAug = false;
        augFrames = INIT_FRAMES;
        curFrames = 0;
    }

    private void NoticeDown()
    {
        oriMaterial = curObject.GetComponent<Renderer>().material;
        print(oriMaterial.name);
        curObject.GetComponent<Renderer>().material = redMaterial;
        scaleAug = false;
        augFrames = INIT_FRAMES;
        curFrames = 0;
    }

    private void NoticeUp()
    {
        curObject.transform.localScale = minScale;
        curObject.layer = norLayer;
        curObject.GetComponent<Renderer>().material = oriMaterial;
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        curObject.layer = augLayer;
        print(curObject.transform.name);
        writer.WriteLine("Noticed" + " " + Time.time);
        writer.Flush();
        
        minScale = new Vector3(curObject.transform.localScale.x, curObject.transform.localScale.y, curObject.transform.localScale.z);
        maxScale = new Vector3(1.5f * curObject.transform.localScale.x, 1.5f * curObject.transform.localScale.y, 1.5f * curObject.transform.localScale.z);

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
            // Random time interval before starting animation
            augTimer = UnityEngine.Random.Range(5, 15);
            isAug = true;
            scaleAug = false;
            curFrames = 0;
            NextLayout();
        }
        if (augTimer > 0)
        {
            augTimer -= Time.deltaTime;
        }
        if (augTimer <= 0 && isAug)
        {
            isAug = false;
            scaleAug = true;
            writer.WriteLine(curObject.transform.name + " " + Time.time);
            writer.Flush();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            NoticeDown();
            NoticeUp();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            NextLayout();
        }
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (scaleAug)
        {   
            // waitTimer is a random time interval to prevent the sudden change between scaling up and down
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            if (waitTimer <= 0 && isWait)
            {
                isWait = false;
                Vector3 temp = oriScale;
                oriScale = tarScale;
                tarScale = temp;
            }
            if (!isWait)
            {
                string t = "Camera: " + Vector3ToString(camera.transform.position) + " " + QuaternionToString(camera.transform.rotation) + " " + Time.time;
                writer.WriteLine(t);
                writer.Flush();

                curFrames = (curFrames + 1) % (augFrames + 1);
                float interpolationRatio = (float) curFrames / augFrames;
                Vector3 interpolatedScale = Vector3.Lerp(oriScale, tarScale, interpolationRatio);
                curObject.transform.localScale = interpolatedScale;
                // if (interpolationRatio == 1)
                // if (Mathf.Approximately(interpolationRatio, 1f))
                if (Mathf.Approximately(curFrames, augFrames))
                {
                    if (tarScale.x < oriScale.x)
                    {
                        augFrames = (int) (augFrames * 0.8f);
                    }
                    curFrames = -1;
                    isWait = true;
                    waitTimer = UnityEngine.Random.Range(1, 3);
                }
            }
        }

        // record gaze data
        if (ifGaze && eyeTrackingData.GazeRay.IsValid)
        {
            string g = "Gaze: " + Vector3ToString(eyeTrackingData.GazeRay.Origin) + " " + Vector3ToString(eyeTrackingData.GazeRay.Direction) + " " + Time.time;
            writer.WriteLine(g);
            writer.Flush();
            int layerMask = 1 << 6 | 1 << 7;
            RaycastHit hit;
            if (Physics.Raycast(eyeTrackingData.GazeRay.Origin, eyeTrackingData.GazeRay.Direction, out hit, Mathf.Infinity, layerMask))
            {
                if (Equals(hit.transform, curObject.transform))
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0f;
                }
            }
        }
        if (timer >= 1.5f && scaleAug)
        {
            NoticeDown();
        }
        if (timer >= 1.55f)
        {
            NoticeUp();
            timer = 0f;
        }
    }
}
