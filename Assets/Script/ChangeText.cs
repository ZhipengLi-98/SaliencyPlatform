using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class ChangeText : MonoBehaviour
{
    public SteamVR_Action_Boolean nextText;

    public SteamVR_Input_Sources controller;
    
    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
        nextText.AddOnStateDownListener(TriggerDown, controller);
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        tmp.text = "Down";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
