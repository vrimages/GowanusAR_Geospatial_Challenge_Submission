using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using static UnityEditor.PlayerSettings;

public class NumGen : MonoBehaviour
{

    //private TextMeshProUGUI text;
    public GameObject textPrefab;

    private float time;
    private float nextActionTime = 0f;
    public float period = 0.1f;

    private GameObject thisPrefab;

    public GameObject target;
    public GameObject cube;

    private List<GameObject> myNums = new List<GameObject>();

    public Transform minx;
    public Transform maxx;
    public Transform minz;
    public Transform maxz;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > nextActionTime){
            nextActionTime = Time.time + period;
            //Debug.Log("hello");
            //float x = Random.Range(-1f, 1f);
            //float z = Random.Range(-1f, 1f);

            float x = Random.Range(minx.position.x, maxx.position.x);
            float z = Random.Range(minz.position.z, maxz.position.z);

            Vector3 pos = new Vector3(x, minx.position.y, z);

            Vector3 randomRotation = new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0,360));
            //thisPrefab = Instantiate(textPrefab, pos, Quaternion.identity);
            thisPrefab = Instantiate(textPrefab, pos, Quaternion.Euler(randomRotation));
            float randomScale = Random.Range(0.5f, 2.0f);
            thisPrefab.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            thisPrefab.GetComponent<Rigidbody>().velocity = new Vector3(0, -10, 0);
            Debug.Log(pos);
            //thisPrefab = Instantiate(textPrefab, this.transform.position, Quaternion.Euler(randomRotation));

            int numText = Random.Range(0,10);
            //thisPrefab.GetComponent<TextMeshProUGUI>().text = numText.ToString();
            thisPrefab.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(numText.ToString());

            //thisPrefab.transform.GetChild(0).GetComponent<Rigidbody>().drag = Random.Range(3.0f, 6.0f);
            //thisPrefab.GetComponent<Rigidbody>().drag = Random.Range(5.0f, 8.0f);
            
            myNums.Add(thisPrefab);
            //thisPrefab.transform.GetChild(0).RotateAround(thisPrefab.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        foreach(GameObject numPrefab in myNums){
            if(numPrefab != null){
                Vector3 rotateDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0,360));
                //numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, Vector3.up, 20 * Time.deltaTime);
                numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, rotateDirection, Random.Range(70,110) * Time.deltaTime);
            }
            
        }

        //cube.transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);

        /*time += Time.deltaTime;

        if(time % 0.1f == 0f){
            Debug.Log("hello");
        }*/
    }
}
