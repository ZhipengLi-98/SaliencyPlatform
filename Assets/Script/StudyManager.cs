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
        currAnim.init();

        m_currCondition++; 
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
