using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    public GameObject door;
    public bool CrowBarObtained;
    private static bool Open;
    private float rotation = 0;
    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        CrowBarObtained = false;
        Open = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player") && CrowBarObtained)
        {
            Open = true;
        }
    }
    private void Update()
    {
        if (Open && rotation < 130)
        {
            rotation += Time.deltaTime * rotationSpeed;
        }
        door.transform.localEulerAngles = new Vector3(door.transform.localEulerAngles.x, rotation, door.transform.localEulerAngles.z);
    }
}
