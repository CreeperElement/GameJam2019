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
    
    //Camera
    private static GameObject[] cameras;
    private GameObject cam;
    private GameObject previousCamera;
    private static GameObject copyObj;
   

    // Door controller, has all relevant gameObject info
    private DoorController controller;

    // Called on startup
    void Start() {
        controller = gameObject.GetComponent<DoorController>();
        InstantiateCameras();
        if (copyObj == null)
            copyObj = new GameObject("Transform");
    }

    private static void InstantiateCameras()
    {
        cameras = cameras == null ? new GameObject[BluePrints.MAX_RENDER_LAYERS] : cameras;
        for(int i = 0; i < BluePrints.MAX_RENDER_LAYERS; i++)
        {
            cameras[i] = (cameras[i] == null) ? Instantiate(BluePrints.CameraPrefab) : cameras[i];
            cameras[i].GetComponent<Camera>().depth = 0-(i+1);
            cameras[i].SetActive(false);
        }
    }

    // Since we have a rendering script here, we might not 
    // need to to any 'physics' updates.
    void Update() { }

    public void OnWillRenderObject()
    {
        startRender();
        if (isValidCamera(cam) && renderedLayers <= BluePrints.MAX_RENDER_LAYERS)// Replace with call determining if the portal should be recursively rendered
        {
            MiniTransform relativeTransform = GetRelativeTransform(previousCamera.transform, controller.forwardPortal.transform, controller.backPortal.transform);
            // Set the transform
            cam.transform.position = relativeTransform.position;
            cam.transform.eulerAngles = relativeTransform.rotation;
            cam.GetComponent<Camera>().Render();
        }
        //cam.GetComponent<Camera>().
        endRender();
    }

    bool isValidCamera(GameObject camera)
    {
        if (camera == null || !camera.tag.ToLower().Equals("recursivecamera"))
            return false;
        Transform doorTrans = controller.forwardPortal.transform;
        Vector3 weights = getPositionalWeights(camera.transform.position - doorTrans.position, doorTrans);

        return FloatingMath.GreaterThan(weights.y, 0f, .01f);
    }

    /// <summary>
    /// Keep track of number of layers, and set the camera
    /// </summary>
    private void startRender()
    {
        if (Camera.current.tag.Equals("RecursiveCamera"))
        {
            previousCamera = Camera.current.gameObject;
            float CameraCount = Camera.current.depth * -1 + 1;    //Start from zero
            if (CameraCount < BluePrints.MAX_RENDER_LAYERS)      // We can make another camera
            {
                cam = cameras[(int)CameraCount - 1];
                cam.SetActive(true);
            }
            renderedLayers++;
        } else
        {
            cam = null;
        }
    }

    /// <summary>
    /// Keep track of the layers
    /// </summary>
    private void endRender()
    {
        // Keep track of current number of renders happening
        renderedLayers--;
        //cam.SetActive(false);
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
        Transform copy = copyObj.transform;

        copy.position = originalObject.forward + originalObject.transform.position;
        Vector3 relativeForward = GetRelativePosition(copy, anchor, newAnchor);

        copy.position += originalObject.forward + originalObject.transform.up;
        Vector3 relativeUp = GetRelativePosition(copy, anchor, newAnchor);

        copy.position = newPosition;
        copy.LookAt(relativeForward, relativeUp-newPosition);

        Vector3 eulerAngles = copy.eulerAngles;
        
        return eulerAngles;
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