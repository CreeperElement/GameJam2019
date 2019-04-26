using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MaxSpeed;
    public float Acceleration;
    public float RotationMultiplier;

    private Rigidbody rbody;
    private GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rbody = GetComponent<Rigidbody>();
        camera = transform.Find("Main Camera").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Create an acceleration
        Vector3 movement = Vector3.zero;

        float forward = Input.GetAxis("Vertical");
        if (Mathf.Abs(forward) > 0)
        {
            movement += transform.forward * forward;
        }
        float side = Input.GetAxis("Horizontal");
        if (Mathf.Abs(side) > 0)
        {
            movement += transform.right * side;
        }

        movement = movement.normalized * Acceleration;
        if (rbody.velocity.magnitude < MaxSpeed)
        {
            rbody.AddForce(movement, ForceMode.Acceleration);
        }

        float mouseX = Input.GetAxis("Mouse X");
        transform.eulerAngles += new Vector3(0, mouseX * RotationMultiplier, 0);
    }
}
