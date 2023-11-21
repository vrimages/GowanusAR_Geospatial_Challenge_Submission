using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using static UnityEditor.PlayerSettings;
//using Niantic.ARDK.Networking;

public class NumGen2 : MonoBehaviour
{

    //private TextMeshProUGUI text;
    public GameObject textPrefab;

    private float time;
    private float nextActionTime = 0f;
    private float period = 0.1f;

    private GameObject thisPrefab;

    public GameObject target;
    public GameObject cube;

    public float speedMultiplier = 0.025f;

    private List<GameObject> myNums = new List<GameObject>();

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
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            //Vector3 pos = new Vector3(x, 5.5f, z);

            Vector3 randomRotation = new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0,360));
            //thisPrefab = Instantiate(textPrefab, pos, Quaternion.identity);
            //thisPrefab = Instantiate(textPrefab, pos, Quaternion.Euler(randomRotation));
            Vector3 thisPos = new Vector3(Random.Range(-0.5f, 0.5f) + target.transform.position.x, target.transform.position.y, Random.Range(-0.5f, 0.5f) + target.transform.position.z);
            //thisPrefab = Instantiate(textPrefab, this.transform.position, Quaternion.Euler(randomRotation));
            thisPrefab = Instantiate(textPrefab, thisPos, Quaternion.Euler(randomRotation));

            thisPrefab.transform.GetChild(0).position = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

            int numText = Random.Range(0,10);
            //thisPrefab.GetComponent<TextMeshProUGUI>().text = numText.ToString();
            thisPrefab.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(numText.ToString());

            //thisPrefab.transform.GetChild(0).GetComponent<Rigidbody>().drag = Random.Range(3.0f, 6.0f);
            //thisPrefab.GetComponent<Rigidbody>().drag = Random.Range(5.0f, 8.0f);

            // store speed value in empty child's x position
            float speed = Random.Range(0.01f, 0.05f);
            //thisPrefab.transform.GetChild(1).position = new Vector3(speed, 0, 0);

            // apply force
            //thisPrefab.GetComponent<Rigidbody>().AddForce(transform.forward * speed * speedMultiplier, ForceMode.Force);

            //myNums.Add(thisPrefab);
            //thisPrefab.transform.GetChild(0).RotateAround(thisPrefab.transform.position, Vector3.up, 20 * Time.deltaTime);
        }

        // rotation
        foreach(GameObject numPrefab in myNums){
            Vector3 rotateDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0,360));
            //numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, Vector3.up, 20 * Time.deltaTime);
            numPrefab.transform.GetChild(0).RotateAround(numPrefab.transform.position, rotateDirection, Random.Range(70,110) * Time.deltaTime);

            float speed = numPrefab.transform.GetChild(1).position.x;
            //numPrefab.GetComponent<Rigidbody>().AddForce(transform.forward * speed * 0.001f, ForceMode.Force);
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
