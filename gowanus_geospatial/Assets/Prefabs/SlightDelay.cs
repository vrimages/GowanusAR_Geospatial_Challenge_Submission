using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlightDelay : MonoBehaviour
{
    float time = 0f;

    public GameObject head;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time>1.5f){
            
            head.SetActive(true);
            this.gameObject.GetComponent<SlightDelay>().enabled = false;
        }
    }
}
