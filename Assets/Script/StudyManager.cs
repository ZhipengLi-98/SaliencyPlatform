using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StudyCondition
{
    public AnimationManager.Background background;
    public bool isVideo;
    public AnimationManager.ChangeAnimation animation; 
}
public class StudyManager : MonoBehaviour
{
    public static StudyManager Study; 

    public List<StudyCondition> m_conditions = new List<StudyCondition>();
    public int m_currCondition; 
    public List<AnimationManager> m_changeAnimations = new List<AnimationManager>();
    private StudyCondition m_currAnim;
    [SerializeField]
    private bool m_autoNext;
    public bool isAutoNext
    {
        get { return m_autoNext; }
    }
    [SerializeField]
    private int m_conditionTrialNum; 
    public int totalTrialNum
    {
        get { return m_conditionTrialNum; }
    }

    public void nextCondition()
    {
        foreach (AnimationManager anim in m_changeAnimations)
        {
            anim.enabled = false; 
        }
        if (m_currCondition >= m_conditions.Count)
        {
            return; 
        }
        StudyCondition condition = m_conditions[m_currCondition];
        m_currAnim = condition;
        AnimationManager currAnim = m_changeAnimations[(int)condition.animation];
        currAnim.curBackground = condition.background;
        currAnim.isVideo = condition.isVideo;
        currAnim.enabled = true;

        if (Logging.Log != null)
        {
            Logging.Log.logCondition(
                m_currCondition,
                condition.background,
                condition.isVideo,
                condition.animation);
        }

        currAnim.init();

        m_currCondition++; 
    }

    // Start is called before the first frame update
    void Start()
    {
        Study = this; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            nextCondition();
        }
    }
}
