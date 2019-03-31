using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetKey : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && GetComponent<Safe>().CrowBarObtained)
        {
            Destroy(GameObject.Find("key"));
            Destroy(GameObject.Find("Exit"));
        }
    }
}
