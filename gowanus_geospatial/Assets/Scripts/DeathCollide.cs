using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollide : MonoBehaviour
{
    private void OnTriggerEnter(Collider c){
        Destroy(c.transform.parent.gameObject);
    }
}
