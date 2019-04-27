using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Automatically does Portal rendering whenever the gameObject is being rendered.
 */
[RequireComponent(typeof(DoorController))]//Make sure this object has a door controller
public class PortalRenderer : MonoBehaviour
{
    /** Keep track of 'how deep' we have rendered our portals.
        If we aren't careful, we can do infinite layers.
     */
    private static int renderedLayers;                  // How many layers deep are we right now?
    public static int MAX_RENDER_LAYERS = 8;      // What is the max number of layers we can go?

    public GameObject reflection;
    public GameObject cameraPrefab;

    //Camera
    Camera cam;

    // Door controller, has all relevant gameObject info
    private DoorController controller;


    // Called on startup
    void Start() {
        controller = gameObject.GetComponent<DoorController>();
    }

    // Since we have a rendering script here, we might not 
    // need to to any 'physics' updates.
    void Update() { }

    public void OnWillRenderObject()
    {
        startRender();
        // Don't go too deep
        if(renderedLayers >= MAX_RENDER_LAYERS || cam == null || cam.name =="SceneCamera" || 90 - Vector3.Angle(cam.transform.forward, controller.forwardPortal.transform.forward) >= 90 )
        {
            endRender();
            return;
        }
        getReflectionCamera(cam, controller);
        endRender();
    }

    // Housekeeping
    private void startRender()
    {
        // Keep track of current number of renders happening
        renderedLayers++;
        cam = Camera.current; // Which camera is currently rendering me?
    }

    // House keeping
    private void endRender()
    {
        // Keep track of current number of renders happening
        renderedLayers--;
    }

    private void getReflectionCamera(Camera original, DoorController controller)
    {

        // Clone the original camera
        Camera reflectionCam = new Camera();
        //reflectionCam.CopyFrom(original);
        GameObject door = controller.forwardPortal;
        GameObject twinner = controller.backPortal;

        Transform doorTransform = controller.getTransform();
        Vector3 doorPosition = door.transform.position;

        Vector3 localCameraOffset = door.transform.position - cam.transform.position;   // Vector from door to camera. Where forward is the door to cam
        float forward = Vector3.Project(localCameraOffset, doorTransform.forward.normalized).magnitude;     //Forward relative offset
        float right = Vector3.Project(localCameraOffset, doorTransform.right.normalized).magnitude;
        float up = Vector3.Project(localCameraOffset, doorTransform.up.normalized).magnitude;

        right = Vector3.SignedAngle(localCameraOffset, door.transform.forward, door.transform.up) > 0 ?
            -1 * right : right;
        forward = Vector3.SignedAngle(localCameraOffset, door.transform.up, door.transform.right) > 0 ?
            forward : -1 * forward;
        up = Vector3.SignedAngle(localCameraOffset, door.transform.right, door.transform.forward) > 0 ?
            -1 * up : up;


        Vector3 normalizedOffsetScalar = new Vector3(forward, up, right);

        //forward * doorTransform.forward + right * doorTransform.right + up * doorTransform.up
        Vector3 newCamPosition = twinner.transform.position + (
                twinner.transform.forward * normalizedOffsetScalar.x +
                twinner.transform.up * normalizedOffsetScalar.y +
                twinner.transform.right * normalizedOffsetScalar.z
            );

        float yRot = -90 - 1*Vector3.SignedAngle(cam.transform.forward, -door.transform.right, cam.transform.up);
        float xRot = 90 - Vector3.SignedAngle(cam.transform.forward, -door.transform.forward, cam.transform.right);
        float zRot = -1*Vector3.SignedAngle(cam.transform.up, door.transform.forward, cam.transform.forward);

        Vector3 newRotation = new Vector3(xRot, yRot, zRot);

        Debug.Log(newRotation);

        // Calculations work up to here, I made some mistakes in the axes, but they offset each other
        
        reflection = Instantiate(cameraPrefab);

        reflection.transform.position = newCamPosition;
        reflection.transform.localEulerAngles = newRotation;

        reflection.GetComponent<Camera>().Render();
        Destroy(reflection);
    }
}
