using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (GameObject obj in objects)
            {
                obj.transform.position = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
            }
        }
    }
}
