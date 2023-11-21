using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Google.XR.ARCoreExtensions;

using Unity.XR.CoreUtils;

using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Rendering.Universal;




public class EnterCrane : MonoBehaviour, IPointerClickHandler
{

    public ContentManager cManager;

    public AREarthManager eManager;


    private SiteData _siteData;

    private GameObject textPanel;
    
    private TextMeshProUGUI text;


    Quaternion from = Quaternion.Euler(0f,0f,0f);
    Quaternion to = Quaternion.Euler(0f,0f,0f);
    float speed = 0.03f;
    float timeCount = 0.0f;

    public Material blue;

    // Start is called before the first frame update
    void Start()
    {
        cManager = GameObject.Find("ContentManager").GetComponent<ContentManager>();
        eManager = FindObjectOfType<XROrigin>(true).gameObject.GetComponent<AREarthManager>();

        _siteData = GameObject.Find("ContentManager").GetComponent<SiteData>();

        textPanel = GameObject.Find("TextCanvas");
        text = textPanel.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        //GameObject.Find("XROrigin").GetComponent<AREarthManager>();

        this.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        this.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y + Random.Range(-0.05f, 0.05f), this.transform.localScale.z);

        this.transform.rotation = Quaternion.Euler(new Vector3(this.transform.rotation.eulerAngles.x, Random.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));
        from = this.transform.rotation;
        to = Quaternion.Euler(new Vector3(this.transform.rotation.eulerAngles.x, Random.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));
    }

    // Update is called once per frame
    void Update()
    {
        if(to != from){
            this.transform.rotation = Quaternion.Lerp(from, to, timeCount * speed);
        }else{
            timeCount = 0f;
            from = this.transform.rotation;
            to = Quaternion.Euler(new Vector3(this.transform.rotation.eulerAngles.x, Random.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z));
        }
        
        timeCount = timeCount + Time.deltaTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //int number = 0;
        //bool success = int.TryParse(this.gameObject.tag, out number);
        //text.text = "Enter " + _siteData.siteData[number] + " Content Point?";
        textPanel.transform.GetChild(0).gameObject.SetActive(true);
        text.text = "Enter " + _siteData.siteData[int.Parse(this.gameObject.tag)] + " Content Point?";

        cManager.tagNum = int.Parse(this.gameObject.tag);

        for (int x=0; x<this.transform.childCount; x++)
        {
            print("clicked");
            if(this.transform.GetChild(x).GetComponent<MeshRenderer>() != null)
            {
                this.transform.GetChild(x).GetComponent<MeshRenderer>().material = blue;
                //.color = Color.blue;
            }
        }

        

        
    }

    /*private void OnTriggerEnter(Collider other){
        
        cManager.enterAR();

        this.GetComponent<BoxCollider>().enabled = false;

        /*
        while(eManager.EarthTrackingState != TrackingState.Tracking){

        }*/
        
        /*
        for(int i=0;i<10;i++){
            if(this.gameObject.tag == i.ToString()){
                if(cManager.nestedList[i].sampleList != null){
                     for(int j=0;j<cManager.nestedList[i].sampleList.Count;j++){
                        Instantiate(cManager.nestedList[i].sampleList[j]);
                    }
                }
    
            }
        }
        

        
    }*/
}
