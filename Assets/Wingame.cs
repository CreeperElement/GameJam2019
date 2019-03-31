using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wingame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Door")){
                if(obj.GetComponent<DoorController>().DoorPairTag.Equals("Win"))
                {
                    Destroy(obj);
                    GameObject.Find("FollowingCamera").GetComponent<FollowingCamera>().enabled = false;

                }
            }
        }
    }
}
