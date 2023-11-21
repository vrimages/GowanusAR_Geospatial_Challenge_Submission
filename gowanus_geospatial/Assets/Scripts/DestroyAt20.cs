using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAt20 : MonoBehaviour
{
    private float time = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time+=Time.deltaTime;
        if(time>20f){
            Destroy(this.gameObject);
        }
    }
}
