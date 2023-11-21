using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using static UnityEditor.PlayerSettings;
//using Niantic.ARDK.Networking;

public class NumGen3 : MonoBehaviour
{

    //private TextMeshProUGUI text;
    public GameObject textPrefab;

    private float time;
    private float nextActionTime = 0f;
    public float period = 0.1f;

    private GameObject thisPrefab;

    public GameObject target;
    public GameObject cube;

    public float speedMultiplier = 0.025f;

    private List<GameObject> myNums = new List<GameObject>();

    private float rotationSpeed;

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
            //Vector3 pos = new Vector3(x, 5.5f, z);

            float x = Random.Range(minx.position.x, maxx.position.x);
            float z = Random.Range(minz.position.y, maxz.position.y);

            Vector3 pos = new Vector3(x, z, minx.position.z);

            Vector3 randomRotation = new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0,360));
            //thisPrefab = Instantiate(textPrefab, pos, Quaternion.identity);
            //thisPrefab = Instantiate(textPrefab, pos, Quaternion.Euler(randomRotation));
            //Vector3 thisPos = new Vector3(Random.Range(-90f, -50f) /*+ target.transform.position.x*/, target.transform.position.y, /*Random.Range(-0.8f, 0.8f) +*/ Random.Range(480f, 520f) /*+ target.transform.position.z*/);
            //thisPrefab = Instantiate(textPrefab, this.transform.position, Quaternion.Euler(randomRotation));
            thisPrefab = Instantiate(textPrefab, pos, Quaternion.Euler(randomRotation));

            float randomScale = Random.Range(0.1f, 1.0f);
            thisPrefab.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            //thisPrefab.transform.GetChild(0).position = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

            int numText = Random.Range(0,10);
            //thisPrefab.GetComponent<TextMeshProUGUI>().text = numText.ToString();
            thisPrefab.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(numText.ToString());

            //thisPrefab.transform.GetChild(0).GetComponent<Rigidbody>().drag = Random.Range(3.0f, 6.0f);
            thisPrefab.GetComponent<Rigidbody>().drag = Random.Range(5.0f, 8.0f);

            // store speed value in empty child's x position
            float speed = Random.Range(0.01f, 0.02f);
            thisPrefab.transform.GetChild(1).position = new Vector3(speed, 0, 0);

            // apply force
            thisPrefab.GetComponent<Rigidbody>().AddForce(transform.forward * speed * speedMultiplier * 0.01f, ForceMode.Force);

            myNums.Add(thisPrefab);
            //thisPrefab.transform.GetChild(0).RotateAround(thisPrefab.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        // rotation
        foreach(GameObject numPrefab in myNums){
            if(numPrefab != null){

                Vector3 rotateDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0,360));

                rotationSpeed = Random.Range(25, 1000);

                numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
                //numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, rotateDirection, Random.Range(70,110) * Time.deltaTime);

                float speed = Random.Range(0.01f, 0.02f);
                //numPrefab.transform.GetChild(1).position.x;
                numPrefab.GetComponent<Rigidbody>().AddForce(transform.forward * speed * speedMultiplier * 0.0003f, ForceMode.Impulse);

            }
            
            //Vector3 movePosition = new Vector3()
            //numPrefab.transform.position = new Vector3(numPrefab.transform.position.x + numPrefab.transform.GetChild(0).position.x, numPrefab.transform.position.y, numPrefab.transform.position.z);
        }

        //cube.transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);

        /*time += Time.deltaTime;

        if(time % 0.1f == 0f){
            Debug.Log("hello");
        }*/
    }
}
