using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;

public class ContentManager : MonoBehaviour
{
    //[SerializeField]
    //public List<List<GameObject>> ListOfLists = new List<List<GameObject>>();

    public List<GameObject> mapAssets = new List<GameObject>();
    public List<GameObject> aRAssets = new List<GameObject>();

    public int tagNum;


    /*public GameObject xROrigin;
    public GameObject aRSession;
    public GameObject geospatial1;
    public GameObject geospatial2;*/


    [System.Serializable]
    public class serializableClass
    {
        public List<GameObject> sampleList;
    }
    public List<serializableClass> nestedList = new List<serializableClass>();


    private List<GameObject> arContent = new List<GameObject>();

    public GameObject gaiaCanvas;


    public void enterAR(){

        arContent.Clear();

        Debug.Log("test");
        for(int x=0;x<mapAssets.Count;x++){
            if(mapAssets[x] != null)
                mapAssets[x].SetActive(false);
        }

        for(int y=0;y<aRAssets.Count;y++){
            if(aRAssets[y] != null)
                aRAssets[y].SetActive(true);
        }

        
        
        var cranes = FindObjectsOfType<EnterCrane>();

        //cranes[tagNum];

        foreach(EnterCrane crane in cranes){
            crane.gameObject.SetActive(false);
        }

        if(tagNum == 1){
            gaiaCanvas.SetActive(true);
        }

        if(nestedList[tagNum].sampleList != null){
                for(int j=0;j<nestedList[tagNum].sampleList.Count;j++){
                GameObject newObj = Instantiate(nestedList[tagNum].sampleList[j]);
                arContent.Add(newObj);
            }
        }
    }

    public void exitAR(){
        for(int x=0;x<mapAssets.Count;x++){
            if(mapAssets[x] != null)
                mapAssets[x].SetActive(true);
        }

        if(tagNum == 1){
            gaiaCanvas.SetActive(false);
        }

        for(int y=0;y<aRAssets.Count;y++){
            if(aRAssets[y] != null)
                aRAssets[y].SetActive(false);
        }

        
        var cranes = FindObjectsOfType<EnterCrane>(true);

        foreach(EnterCrane crane in cranes){
            crane.gameObject.SetActive(true);
        }

        var gameObjects = FindObjectsOfType<ARGeospatialCreatorAnchor>();

        foreach(ARGeospatialCreatorAnchor obj in gameObjects){
            Destroy(obj.gameObject);
        }

        //var layerObjects = FindGameObjectsWithLayer("Content");
        foreach(GameObject g in arContent){
            Destroy(g);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
