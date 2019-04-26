using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowbar : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            GameObject.Find("Safe (1)").GetComponent<Safe>().CrowBarObtained = true;
            GameObject.Find("Safe").GetComponent<Safe>().CrowBarObtained = true;
            Destroy(gameObject);
        }
    }
}
