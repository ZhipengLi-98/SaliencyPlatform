using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class AnimationManager : MonoBehaviour
{
    public GameObject VirtualHome;
    public GameObject VirtualLab;
    public GameObject VirtualCafe;
    public GameObject PhysicalHome1;
    public GameObject PhysicalHome2;
    public GameObject PhysicalHome3;

    public enum ChangeAnimation
    {
        Position, Scale, Color
    }

    public enum Background
    {
        VirtualHome, VirtualLab, VirtualCafe, PhysicalHome1, PhysicalHome2, PhysicalHome3
    }
    public Background curBackground;

    public bool isVideo;

    //protected StreamWriter writer;

    public virtual void init()
    {

    }

    protected int trialNum;

    public delegate void IsNextCondition(int trialNum);
    public IsNextCondition isNextCondition; 
}
