using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class FishClick : MonoBehaviour, IPointerClickHandler/*, IPointerDownHandler, IPointerUpHandler*///, IPointerClickHandler
{

    //public AudioSource audio;

    private AudioSource audio;

    AudioSource[] audios;

    public Material blue;

    // Start is called before the first frame update
    void Start()
    {
        audio = this.GetComponent<AudioSource>();

        //this.GetComponent<SpriteRenderer>().material = blue;

        audios = (AudioSource[]) GameObject.FindObjectsOfType (typeof(AudioSource));
    }


    //Detect if a click occurs
    /*public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        Debug.Log(name + " Game Object Clicked!");
    }*/

    

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Test1");
        
        foreach(AudioSource a in audios){
            a.Stop();
        }

        if(audio.isPlaying){
            //audio.Stop();
        }else{
            audio.Play();
        }
        
        this.GetComponent<SpriteRenderer>().material = blue;
    }

    /*
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Test2");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Test3");
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("hi");

        }


        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Debug.Log("yo");
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                Debug.Log("Something Hit");

                /*if (raycastHit.collider.CompareTag("Tropfen"))
                {
                    Debug.Log("object clicked");
                    TropfenObject.GetComponent<Tropfen>().TropfenDestruction();
                }*/
            /*}
        }
    }*/
}
