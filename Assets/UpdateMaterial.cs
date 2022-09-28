using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
        {
            Renderer temp = t.gameObject.GetComponent<Renderer>();
            if (temp != null)
            {
                temp.material = this.GetComponent<Renderer>().material;
            }
        }
    }
}
