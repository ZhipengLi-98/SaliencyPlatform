using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Tobii.XR;

public class Logging : MonoBehaviour
{
    public static Logging Log;

    private StreamWriter m_swConditions;
    private StreamWriter m_swTracked;
    private StreamWriter m_swNoticeability;
    private StreamWriter m_swLayouts;

    private System.DateTime m_tStart;
    private double m_t
    {
        get { return (System.DateTime.UtcNow - m_tStart).TotalSeconds; }
    }

    // Total executed conditions 
    private int m_currCondition;
    // Trial in current condition 
    private int m_currConditionTrial;
    // Total executed trials 
    private int m_currTrial; 

    [Header("Log Identification")]
    [SerializeField]
    private string m_logDirectory;
    [SerializeField]
    private int m_id;

    [Header("Logged Features")]
    [SerializeField]
    private Transform m_camera;
    [SerializeField]
    private Valve.VR.InteractionSystem.Hand m_lHand;
    [SerializeField]
    private Valve.VR.InteractionSystem.Hand m_rHand; 
        

    string Vector3ToString(Vector3 v)
    {
        string res = v.x + "," + v.y + "," + v.z;
        return res;
    }

    string QuaternionToString(Quaternion q)
    {
        string res = q.x + "," + q.y + "," + q.z + "," + q.w;
        return res;
    }

    private void reset()
    {
        if (m_swConditions != null && m_swConditions.BaseStream != null)
        {
            m_swConditions.Close();
            m_swConditions = null;
        }

        if (m_swTracked != null && m_swTracked.BaseStream != null)
        {
            m_swTracked.Close();
            m_swTracked = null;
        }

        if (m_swNoticeability != null && m_swNoticeability.BaseStream != null)
        {
            m_swNoticeability.Close();
            m_swNoticeability = null;
        }

        if (m_swLayouts != null && m_swLayouts.BaseStream != null)
        {
            m_swLayouts.Close();
            m_swLayouts = null;
        }

        m_currCondition = 0;
        m_currConditionTrial = 0;
        m_currTrial = 0; 
    }

    private void init()
    {
        reset();

        m_tStart = System.DateTime.Now;

        string fid = m_id.ToString() + "-" 
            + System.DateTime.Now.Month.ToString("D2") + "-"
            + System.DateTime.Now.Day.ToString("D2") + "-"
            + System.DateTime.Now.Hour.ToString("D2") + "-"
            + System.DateTime.Now.Minute.ToString("D2") + "-"
            + System.DateTime.Now.Second.ToString("D2");
        m_swConditions = new StreamWriter(m_logDirectory + "/" + "conditions" + "-" + fid + ".csv");
        m_swTracked = new StreamWriter(m_logDirectory + "/" + "tracked" + "-" + fid + ".csv");
        m_swNoticeability = new StreamWriter(m_logDirectory + "/" + "noticeability" + "-" + fid + ".csv");
        m_swLayouts = new StreamWriter(m_logDirectory + "/" + "layouts" + "-" + fid + ".csv");
    }

    public void logCondition(
        int conditionID,
        AnimationManager.Background background, 
        bool isVideo, 
        AnimationManager.ChangeAnimation effect)
    {
        if (m_swConditions == null || m_swConditions.BaseStream == null)
            return; 

        string entry = m_currCondition + "," +
            m_t + "," + 
            conditionID + "," +
            background + ",";
        if (isVideo)
            entry += "Video";
        else
            entry += "Typing";
        entry += ",";
        entry += effect;
        m_swConditions.WriteLine(entry);

        m_currCondition++;
        m_currConditionTrial = 0; 
    }

    private string PoseToString(Transform obj)
    {
        return Vector3ToString(obj.position) + "," +
        Vector3ToString(obj.forward) + "," +
        Vector3ToString(obj.up) + "," +
        QuaternionToString(obj.rotation);
    }

    private string HandToString(Valve.VR.InteractionSystem.Hand hand)
    {
        return hand.isActive + "," +
            hand.isPoseValid + "," +
            PoseToString(hand.transform);
    }

    private void logTracked()
    {
        if (m_swTracked == null || m_swTracked.BaseStream == null)
            return; 

        string entry = m_currCondition + "," +
            m_t + ",";

        // Camera 
        entry += PoseToString(m_camera) + ",";

        // Controllers 
        entry += HandToString(m_lHand) + ",";
        entry += HandToString(m_rHand) + ",";

        // Eye-tracking 
        var eyeTrackingData = TobiiXR.GetEyeTrackingData(TobiiXR_TrackingSpace.World);
        entry += eyeTrackingData.GazeRay.IsValid + "," +
            Vector3ToString(eyeTrackingData.GazeRay.Origin) + "," + 
            Vector3ToString(eyeTrackingData.GazeRay.Direction);

        m_swTracked.WriteLine(entry);

    }

    public void logTrialStart(List<GameObject> elements, 
        Transform videoPlayer, 
        Transform keyboard, 
        string updatedElement, 
        float effectDelay,
        Vector3 effectMinScale, 
        Vector3 effectMaxScale,
        Vector3 effectMinPosition,
        Vector3 effectMaxPosition,
        float effectHue,
        int augFrames)
    {
        if (m_swNoticeability == null || m_swNoticeability.BaseStream == null)
            return;

        Debug.Log("Updated Element: " + updatedElement + ", " +
            "Delay: " + effectDelay);

        // Trial details 
        string entry = m_currCondition + "," +
            m_currConditionTrial + "," +
            m_currTrial + "," +
            m_t + "," +
            "start" + "," +
            updatedElement + "," +
            effectDelay + "," + 
            Vector3ToString(effectMinScale) + "," + 
            Vector3ToString(effectMaxScale) + "," + 
            Vector3ToString(effectMinPosition) + "," +
            Vector3ToString(effectMaxPosition) + "," +
            effectHue + "," + 
            augFrames;
        m_swNoticeability.WriteLine(entry);

        if (m_swLayouts == null || m_swLayouts.BaseStream == null)
            return;

        // Trial layout 
        entry = m_currCondition + "," +
            m_currConditionTrial + "," +
            m_currTrial + "," +
            m_t + ",";
        foreach(GameObject element in elements)
        {
            entry += element.name + "," + 
                element.activeInHierarchy + "," +
                PoseToString(element.transform) + ",";
        }
        entry += videoPlayer.name + "," +
                videoPlayer.gameObject.activeInHierarchy + "," +
            PoseToString(videoPlayer.transform) + ",";
        entry += keyboard.name + "," +
                keyboard.gameObject.activeInHierarchy + "," +
            PoseToString(keyboard.transform) + ",";

        m_swLayouts.WriteLine(entry);

    }

    public void logEffectStart()
    {
        if (m_swNoticeability == null || m_swNoticeability.BaseStream == null)
            return;

        string entry = m_currCondition + "," +
            m_currConditionTrial + "," +
            m_currTrial + "," +
            m_t + "," +
            "change";
        m_swNoticeability.WriteLine(entry);
    }

    public void logTrialNoticed()
    {
        if (m_swNoticeability == null || m_swNoticeability.BaseStream == null)
            return;

        string entry = m_currCondition + "," +
            m_currConditionTrial + "," +
            m_currTrial + "," +
            m_t + "," +
            "noticed";
        m_swNoticeability.WriteLine(entry);

        m_currConditionTrial++;
        m_currTrial++; 
    }

    // Start is called before the first frame update
    void Start()
    {
        Log = this; 
        init(); 
    }

    // Update is called once per frame
    void Update()
    {
        logTracked();
    }

    private void OnApplicationQuit()
    {
        reset();
    }
}
