using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    private GameObject player, door1, door2;
    Vector3 camOffset;
    GameObject otherCamera;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        camOffset = new Vector3(0, 2, 0);
        otherCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject playerDoor = findPlayerDoor();
        GameObject otherDoor = playerDoor.GetComponent<DoorController>().OtherDoor;
        
        float playerAngle = Vector2.SignedAngle(
            get2DVector(playerDoor.transform.forward),
            get2DVector(player.transform.position - playerDoor.transform.position));
        playerAngle = 0 - playerAngle;

        float magnitude = Vector2.Distance(get2DVector(otherCamera.transform.position), get2DVector(playerDoor.transform.position));

        float xPlayer = Mathf.Sin(Mathf.PI * playerAngle / 180f);
        float yPlayer = Mathf.Cos(Mathf.PI * playerAngle / 180f);

        transform.position = camOffset + otherDoor.transform.position + otherDoor.transform.right * xPlayer * magnitude + otherDoor.transform.forward * yPlayer * magnitude;
        
        float playerLookAngle = Vector2.SignedAngle(get2DVector(player.transform.forward), get2DVector(playerDoor.transform.forward));

        var xrot = otherCamera.transform.eulerAngles.x;
        var yrot = playerLookAngle + otherDoor.transform.eulerAngles.y;
        var zrot = 0;

        transform.eulerAngles = new Vector3(xrot, yrot, zrot);
    }

    private Vector2 get2DVector(Vector3 original)
    {
        return new Vector2(original.x, original.z);
    }

    private GameObject findPlayerDoor()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        float shortestDist = float.MaxValue;
        int indexOfShortest = 0;
        
        for(int i = 0; i < doors.Length; i++)
        {
            float distance = Vector3.Distance(player.transform.position, doors[i].transform.position);
            if (distance < shortestDist)
            {
                shortestDist = distance;
                indexOfShortest = i;
            }
        }
        return doors[indexOfShortest];
    }
}
