using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour
{

    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenHelp(){
        canvas.SetActive(true);
        print("opencanvas");
    }


    public void CloseHelp(){
        canvas.SetActive(false);
        print("closecanvas");
    }


}
