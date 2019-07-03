using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{

    private bool colliding;
    private MeshRenderer current;

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Portal")) {
            obj.GetComponent<MeshRenderer>().enabled = true;
        }

        if (!colliding)
            current = null;

        if (current != null)
            current.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Portal"))
        {
            colliding = true;
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
    }
}
