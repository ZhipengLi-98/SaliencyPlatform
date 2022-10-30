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
    public List<StudyCondition> m_conditions = new List<StudyCondition>();
    public int m_currCondition; 
    public List<AnimationManager> m_changeAnimations = new List<AnimationManager>();
    public bool m_autoNext;
    public int m_conditionTrialNum; 

    public void isNextCondition(int trialNum)
    {
        if (m_autoNext && trialNum >= m_conditionTrialNum)
            nextCondition(); 
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
        foreach (AnimationManager anim in m_changeAnimations)
        {
            anim.isNextCondition = isNextCondition;
        }
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
