using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoorListener : MonoBehaviour
{
    public bool ContainsPlayer;
    public bool ContainsCamera { get { return child.GetComponent<CamListener>().ContainsCamera; } }
    private static bool IsTeleporting;

    public GameObject child;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            ContainsPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            ContainsPlayer = false;
        }
    }
}
