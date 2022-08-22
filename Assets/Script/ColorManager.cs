using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using System.IO;
using Valve.VR.InteractionSystem;
using Valve.VR;
using System;

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

    public bool scaleAug = false;

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
        // scaleAug = false;
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

        augFrames = INIT_FRAMES;
        curObject = userInterefaces[UnityEngine.Random.Range(0, userInterefaces.Count)];
        curObject.layer = augLayer;
        print(curObject.transform.name);

        layout = new List<Dictionary<string, List<Vector3>>>();
        ReadLayout();
        NextLayout();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
