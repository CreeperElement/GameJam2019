using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float RotationMultipler;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotation = Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(rotation * -1 * RotationMultipler, 0, 0);
    }
}
