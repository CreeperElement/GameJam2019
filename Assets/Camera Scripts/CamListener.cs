using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamListener : MonoBehaviour
{
    public bool ContainsCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("MainCamera"))
        {
            ContainsCamera = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("MainCamera"))
        {
            ContainsCamera = false;
        }
    }
}
