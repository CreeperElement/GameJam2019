using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DoorController : MonoBehaviour
{
    private enum Direction
    {
        Forward, Backward, None
    }

    public string DoorPairTag;

    private GameObject twinner;
    private GameObject player;
    private DoorController twinnerController;
    public GameObject forwardPortal, backPortal;
    private Direction direction;

    private PortalDoorListener frontListener, backListener;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach(GameObject door in doors)
        {
            if (door.GetComponent<DoorController>().DoorPairTag.Equals(this.DoorPairTag) && !door.Equals(gameObject))
            {
                twinner = door;
                twinnerController = twinner.GetComponent<DoorController>();
            }
        }
        player = GameObject.Find("Player");

        frontListener = forwardPortal.GetComponent<PortalDoorListener>();
        backListener = backPortal.GetComponent<PortalDoorListener>();

        direction = Direction.None;
    }

    // Update is called once per frame
    void Update()
    {
        forwardPortal.gameObject.SetActive(Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(twinner.transform.position, player.transform.position));
        backPortal.gameObject.SetActive(Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(twinner.transform.position, player.transform.position));

        // The colliders DO overlap
        if (frontListener.ContainsPlayer) {
        
            switch (direction)
            {
                case Direction.None: // Meaning we just stepped in
                    direction = Direction.Backward; // From front to back = Forward
                    break;
                case Direction.Forward: // From back forward
                    if(!backListener.ContainsPlayer && frontListener.ContainsCamera)
                        swapPlayer(); // We moved all the way forward
                    break;
            } // No other cases, if we are in the forward position, we are either going forward or backward
            forwardPortal.GetComponentInChildren<MeshRenderer>().enabled = !frontListener.ContainsCamera;
        } else
        {
            forwardPortal.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        if (backListener.ContainsPlayer)
        {
            switch (direction)
            {
                case Direction.None: // Meaning we just stepped in
                    direction = Direction.Forward; // From front to back = Forward
                    break;
                case Direction.Backward: // From back forward
                    if(!frontListener.ContainsPlayer && backListener.ContainsCamera)
                        swapPlayer(); // We moved all the way forward
                    break;
            } // No other cases, if we are in the forward position, we are either going forward or backward

            backPortal.GetComponentInChildren<MeshRenderer>().enabled = !backListener.ContainsCamera;
        } else
        {
            backPortal.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        

        if (!frontListener.ContainsPlayer && !backListener.ContainsPlayer)
        {
            direction = Direction.None;
        }
    }

    private void swapPlayer()
    {

        Debug.Log("Swapp");

        if (direction == Direction.Forward)
        {
            swap(backPortal.transform.parent.gameObject, twinnerController.backPortal.transform.parent.gameObject, player);
            twinnerController.direction = Direction.Backward;
        }
        else // Or backward
        {
            swap(forwardPortal.transform.parent.gameObject, twinnerController.forwardPortal.transform.parent.gameObject, player);
            twinnerController.direction = Direction.Forward;
        }

        // Cleanup
        backListener.ContainsPlayer = false;
        frontListener.ContainsPlayer = false;

        twinnerController.backListener.ContainsPlayer = false;
        twinnerController.frontListener.ContainsPlayer = false;
        
    }


    private void swap(GameObject startingDoor, GameObject destinationDoor, GameObject player)
    {
        float playerAngle = Vector2.SignedAngle(
            get2DVector(startingDoor.transform.forward),
            get2DVector(player.transform.position - startingDoor.transform.position));
        playerAngle = 0 - playerAngle;

        float magnitude = Vector2.Distance(get2DVector(player.transform.position), get2DVector(startingDoor.transform.position));

        float xPlayer = Mathf.Sin(Mathf.PI * playerAngle / 180f);
        float yPlayer = Mathf.Cos(Mathf.PI * playerAngle / 180f);

        player.transform.position = destinationDoor.transform.position + destinationDoor.transform.right * xPlayer * magnitude + destinationDoor.transform.forward * yPlayer * magnitude;
        if(player.transform.position.y <= destinationDoor.transform.position.y)
        {
            player.transform.position = new Vector3(player.transform.position.x, destinationDoor.transform.position.y + .01f, player.transform.position.z);
        }

        float playerLookAngle = Vector2.SignedAngle(get2DVector(player.transform.forward), get2DVector(startingDoor.transform.forward));

        var xrot = 0;
        var yrot = GameObject.Find("FollowingCamera").transform.eulerAngles.y;
        var zrot = 0;

        player.transform.eulerAngles = new Vector3(xrot, yrot, zrot);
    }

    private Vector2 get2DVector(Vector3 original)
    {
        return new Vector2(original.x, original.z);
    }

    public GameObject OtherDoor {
        get { return twinner; }
    }
}
