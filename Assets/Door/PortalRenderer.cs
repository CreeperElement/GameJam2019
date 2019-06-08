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
    public static int MAX_RENDER_LAYERS = 1;      // What is the max number of layers we can go?

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
        if (isValidCamera(cam))// Replace with call determining if the portal should be recursively rendered
        {
            MiniTransform relativeTransform = GetRelativeTransform(cam.transform, gameObject.transform, controller.backPortal.transform);
            cameraPrefab.transform.position = relativeTransform.position;
            cameraPrefab.transform.eulerAngles = relativeTransform.rotation;
        }
        endRender();
    }

    bool isValidCamera(Camera cam)
    {
        return cam.tag.ToLower().Equals("recursivecamera");
    }

    /// <summary>
    /// Keep track of number of layers, and set the camera
    /// </summary>
    private void startRender()
    {
        // Keep track of current number of renders happening
        renderedLayers++;
        cam = Camera.current; // Which camera is currently rendering me?
    }

    /// <summary>
    /// Keep track of the layers
    /// </summary>
    private void endRender()
    {
        // Keep track of current number of renders happening
        renderedLayers--;
    }

    /// <summary>
    /// <para>Calculates the relative transform of newAnchor so that it reflects the relationship (Position and Rotation) of Object relative to Anchor.</para>
    /// IE: The positional difference between Object and Anchor will be the same as the returned value and newAnchor.
    /// <bold>TODO: Rotation is not implemented yet, more research needs to be done!</bold>
    /// </summary>
    /// <param name="originalObject">The object whose relatie transform we will be copying.</param>
    /// <param name="anchor">Object's reference point.</param>
    /// <param name="newAnchor">The new reference point</param>
    /// <returns>A tranform reflecting Object's position and rotation relative to the anchor, around the newAnchor</returns>
    private MiniTransform GetRelativeTransform(Transform originalObject, Transform anchor, Transform newAnchor)
    {
        // Get the new location
        Vector3 newPosition = GetRelativePosition(originalObject, anchor, newAnchor);
        // TODO: This is a placeholder. We need to reserarch and implement this bit yet 
        Vector3 newRotation = getRelativeRotation(originalObject, anchor, newAnchor, newPosition);

        return new MiniTransform(newPosition, newRotation);
    }

    /// <summary>
    /// Get the 3D position which is in the same location relative to newAnchor, as originalObject is to anchor.
    /// </summary>
    /// <param name="originalObject"></param>
    /// <param name="anchor"></param>
    /// <param name="newAnchor"></param>
    /// <returns>A vector represetning the relative 3D location of the new Object.</returns>
    public Vector3 GetRelativePosition(Transform originalObject, Transform anchor, Transform newAnchor)
    {
        Vector3 positionalWeights = getPositionalWeights(anchor.position - originalObject.position, anchor);
        return newAnchor.transform.position
                + newAnchor.right * positionalWeights.x
                + newAnchor.up * positionalWeights.y
                + newAnchor.forward * positionalWeights.z;
    }

    /// <summary>
    /// Gets the X,Y,Z weights of the positionalOffset relative to the masterTransform.
    /// </summary>
    /// <param name="positionalOffset"></param>
    /// <param name="masterTransform"></param>
    /// <returns></returns>
    private Vector3 getPositionalWeights(Vector3 positionalOffset, Transform masterTransform)
    {
        float xWeight, yWeight, zWeight;                                                // 3 floats to determine weights
        xWeight = Vector3.Project(positionalOffset, masterTransform.right).magnitude;   // Get the x-component
        yWeight = Vector3.Project(positionalOffset, masterTransform.up).magnitude;      // Get the y-component
        zWeight = Vector3.Project(positionalOffset, masterTransform.forward).magnitude; // Get the z-component

        xWeight = Vector3.SignedAngle(positionalOffset, masterTransform.forward, masterTransform.up) > 0 ? -1 * xWeight : xWeight;
        yWeight = Vector3.SignedAngle(positionalOffset, masterTransform.right, masterTransform.forward) > 0 ? -1 * yWeight : yWeight;
        zWeight = Vector3.SignedAngle(positionalOffset, masterTransform.up, masterTransform.right) > 0 ? zWeight : -1 * zWeight;

        return new Vector3(xWeight, yWeight, zWeight);
    }

    /// <summary>
    /// Takes two trasforms and calculates the relative rotation difference between the two transforms.
    /// </summary>
    /// <param name="originalObject">Satellite object</param>
    /// <param name="anchor">Object to get roation relative of.</param>
    /// <returns>Euler Angles</returns>
    private Vector3 getRelativeRotation(Transform originalObject, Transform anchor, Transform newAnchor, Vector3 newPosition)
    {
        GameObject copyObj = new GameObject();
        Transform copy = copyObj.transform;

        copy.position = originalObject.forward + originalObject.transform.position;
        Vector3 relativeForward = GetRelativePosition(copy, anchor, newAnchor);

        copy.position += originalObject.forward + originalObject.transform.up;
        Vector3 relativeUp = GetRelativePosition(copy, anchor, newAnchor);

        copy.position = newPosition;
        copy.LookAt(relativeForward, relativeUp-newPosition);

        Vector3 eulerAngles = copy.eulerAngles;

        Destroy(copyObj);
        return eulerAngles;
    }

    private void getReflectionCamera(Camera original, DoorController controller)
    {
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

        /*float yRot = -90 - 1*Vector3.SignedAngle(cam.transform.forward, -door.transform.right, cam.transform.up);
        float xRot = 90 - Vector3.SignedAngle(cam.transform.forward, -door.transform.forward, cam.transform.right);
        float zRot = -1*Vector3.SignedAngle(cam.transform.forward, door.transform.forward, cam.transform.right);*/

        float xRot = (90 + door.transform.localEulerAngles.x) + cam.transform.localEulerAngles.x;
        float yRot = doorTransform.localEulerAngles.y + cam.transform.localEulerAngles.y;
        float zRot = doorTransform.localEulerAngles.z + cam.transform.localEulerAngles.z;

        Vector3 newRotation = new Vector3(xRot, yRot, zRot);
            //-door.transform.localEulerAngles - cam.transform.localEulerAngles;//= new Vector3(xRot, yRot, zRot);

        // Calculations work up to here, I made some mistakes in the axes, but they offset each other
        
        reflection = Instantiate(cameraPrefab);
        reflection.name += GameObject.FindGameObjectsWithTag("RecursiveCamera").Length;

        reflection.transform.position = newCamPosition;
        reflection.transform.localEulerAngles = newRotation;

        reflection.GetComponent<Camera>().Render();
        Destroy(reflection, 0f);
    }

}

class MiniTransform
{
    public Vector3 position { get; set; }
    public Vector3 rotation { get; set; }

    public MiniTransform()
    {
        position = Vector3.zero;
        rotation = Vector3.zero;
    }

    public MiniTransform(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

// Something to Consider::
//Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera);
//      if (GeometryUtility.TestPlanesAABB(planes, Object.collider.bounds))
//          return true;
//      else
//          return false;