using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
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
    public List<GameObject> iconList = new List<GameObject>();
    public List<GameObject> viewerList = new List<GameObject>();

    public GameObject camera;

    private int INIT_FRAMES = 300;

    private int augFrames;
    private int curFrames = 0;

    private float timer = 0f;

    public bool colorAug = false;

    private string user = "test.txt";
    private StreamWriter writer;

    private GameObject curObject;

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

    private float curHue = 0f;
    
    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // curObject.transform.localScale = minScale;
        // curObject.layer = norLayer;
        // curObject.GetComponent<Renderer>().material = oriMaterial;
        // curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        // curObject.layer = augLayer;
        // print(curObject.transform.name);
        // writer.WriteLine("Noticed" + " " + Time.time);
        // writer.Flush();
        // if (iconList.Contains(curObject))
        // {
        //     minScale = minScale1;
        //     maxScale = maxScale1;
        // }
        // else
        // {
        //     minScale = minScale2;
        //     maxScale = maxScale2;
        // }
        // oriScale = minScale;
        // tarScale = maxScale;
        // RandomPosition();
        // NextLayout();
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        // oriMaterial = curObject.GetComponent<Renderer>().material;
        // print(oriMaterial.name);
        // curObject.GetComponent<Renderer>().material = redMaterial;
        // colorAug = false;
        // augFrames = INIT_FRAMES;
        // curFrames = 0;
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

    // Start is called before the first frame update
    void Start()
    {
        home.SetActive(isHome);
        lab.SetActive(isLab);
        cafe.SetActive(isCafe);
        
        // notice.AddOnStateUpListener(TriggerUp, controller);
        // notice.AddOnStateDownListener(TriggerDown, controller);
        
        writer = new StreamWriter(user, false);
        
        curHue = Random.Range(0f, 1f);

        augFrames = INIT_FRAMES;
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        curObject.layer = augLayer;
        curObject.GetComponent<Renderer>().material.color = Color.HSVToRGB(curHue, 1.0f, 1.0f);
        print(curObject.transform.name);

        layout = new List<Dictionary<string, List<Vector3>>>();
        ReadLayout();
        NextLayout();
    }

    // Update is called once per frame
    void Update()
    {
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
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
            colorAug = true;
            writer.WriteLine(curObject.transform.name + " " + Time.time);
            writer.Flush();
        }
        if (colorAug)
        {   
        //     if (waitTimer > 0)
        //     {
        //         waitTimer -= Time.deltaTime;
        //     }
        //     if (waitTimer <= 0 && isWait)
        //     {
        //         isWait = false;
        //         Vector3 temp = oriScale;
        //         oriScale = tarScale;
        //         tarScale = temp;
        //     }
        //     if (!isWait)
        //     {
        //         string t = "Camera: " + Vector3ToString(camera.transform.position) + " " + QuaternionToString(camera.transform.rotation) + " " + Time.time;
        //         writer.WriteLine(t);
        //         writer.Flush();

        //         curFrames = (curFrames + 1) % (augFrames + 1);
        //         float interpolationRatio = (float) curFrames / augFrames;
        //         Vector3 interpolatedScale = Vector3.Lerp(oriScale, tarScale, interpolationRatio);
        //         curObject.transform.localScale = interpolatedScale;
        //         if (Mathf.Approximately(curFrames, augFrames))
        //         {
        //             if (tarScale.x < oriScale.x)
        //             {
        //                 augFrames = (int) (augFrames * 0.8f);
        //             }
        //             curFrames = -1;
        //             isWait = true;
        //             waitTimer = UnityEngine.Random.Range(1, 3);
        //         }
        //     }
        }
    }
}
