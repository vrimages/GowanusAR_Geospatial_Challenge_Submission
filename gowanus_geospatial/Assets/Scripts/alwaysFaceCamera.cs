using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alwaysFaceCamera : MonoBehaviour
{
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("ARCam").transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(target);
    }
}
