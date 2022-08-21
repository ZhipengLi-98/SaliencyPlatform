using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System;

public class AugmentationController : MonoBehaviour
{
    public GameObject home;
    public GameObject lab;
    public GameObject cafe;

    public bool isHome;
    public bool isLab;
    public bool isCafe;

    public SteamVR_Action_Boolean notice;

    public SteamVR_Input_Sources controller;

    public List<GameObject> userInterefaces = new List<GameObject>();

    public GameObject camera;

    private int INIT_FRAMES = 300;

    private int augFrames;
    private int curFrames = 0;

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
    public List<GameObject> iconList = new List<GameObject>();
    public List<GameObject> viewerList = new List<GameObject>();

    private GameObject curObject;

    private string user = "djx_typing_con.txt";
    private StreamWriter writer;

    public bool ifGaze = false;

    private int augLayer;
    private int norLayer;

    public Material redMaterial;
    private Material oriMaterial;
    private GameObject redObject;

    public bool isTyping;

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
        home.SetActive(isHome);
        lab.SetActive(isLab);
        cafe.SetActive(isCafe);
        
        augLayer = LayerMask.NameToLayer("AugObj");
        norLayer = LayerMask.NameToLayer("NorObj");
        if (!ifGaze)
        {
            notice.AddOnStateUpListener(TriggerUp, controller);
            notice.AddOnStateDownListener(TriggerDown, controller);
        }

        writer = new StreamWriter(user, false);

        augFrames = INIT_FRAMES;
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        curObject.layer = augLayer;
        print(curObject.transform.name);

        if (iconList.Contains(curObject))
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
        // RandomPosition();
        layout = new List<Dictionary<string, List<Vector3>>>();
        ReadLayout();
        NextLayout();
    }

    private void RandomPosition()
    {
        foreach (GameObject obj in userInterefaces)
        {
            float x1 = UnityEngine.Random.Range(-1.0f, -0.6f);
            float x2 = UnityEngine.Random.Range(0.6f, 1.0f);
            float x = UnityEngine.Random.Range(-1.0f, 1.0f);
            List<float> xs = new List<float>();
            xs.Add(x1);
            xs.Add(x2);
            float y = UnityEngine.Random.Range(0.5f, 1.5f);
            float z = UnityEngine.Random.Range(0.5f, 1.2f);
            if (isTyping)
            {
                obj.transform.position = new Vector3(xs[UnityEngine.Random.Range(0, xs.Count)], y, z);
            }
            else
            {
                obj.transform.position = new Vector3(x, y, z);
            }
            obj.transform.LookAt(camera.transform);
        }
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
        }
        int next = UnityEngine.Random.Range(0, layout.Count);
        while (next == layoutCnt)
        {
            next = UnityEngine.Random.Range(0, layout.Count);
        }
        layoutCnt = next;
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
        if (iconList.Contains(curObject))
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
        // RandomPosition();
        // NextLayout();
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
        if (iconList.Contains(curObject))
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
        // RandomPosition();
        // NextLayout();
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
            augTimer = UnityEngine.Random.Range(5, 15);
            isAug = true;
            // RandomPosition();
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
            scaleAug = false;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // RandomPosition();
            NextLayout();
        }
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        if (scaleAug)
        {   
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            if (waitTimer <= 0 && isWait)
            {
                isWait = false;
                // Vector3 temp = new Vector3(oriScale.x, oriScale.y, oriScale.z);
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
