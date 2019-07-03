using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCutoutRender : MonoBehaviour
{
    public Material mat;
    public Shader spriteShader;
    public DoorController controller;
    public Texture newTexture;

    private Camera cam;

    private void Start()
    {
        //mat = new Material(spriteShader);
        cam = GetComponent<Camera>();
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /* Step 1: We make a plane. Unity allows us to define a plane by defining three points which the plane
         * is allowed to pass through. Theoretically, if we use the forward, up, and right components of the camera
         * in combination with the nearClipPlane distance; we can create three points on the nearClipPlane, and 
         * define the nearClipPlane.
         */

        /*
        Vector3 passThroughA, passThroughB, passThroughC;
        passThroughA = transform.position + transform.forward * cam.nearClipPlane;
        passThroughB = passThroughA + transform.right;
        passThroughC = passThroughA + transform.up;
        Plane nearClipplingPlane = new Plane(passThroughA, passThroughB, passThroughC);

        Mesh mesh = controller.forwardPortal.GetComponent<MeshFilter>().mesh;
        Transform portalTransform = controller.forwardPortal.transform;
        foreach(Vector3 vertex in mesh.vertices)
        {
            // Make a ray
            Vector3 vertexPosition = portalTransform.TransformPoint(vertex);
            Ray ray = new Ray(vertexPosition, transform.position - vertexPosition);

            Vector3 origin = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            Vector3 upperRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.scaledPixelHeight, cam.nearClipPlane));

            // Cast ray to plane
            float enter;
            if(nearClipplingPlane.Raycast(ray, out enter))
            {
                // Find collision point
                Vector3 collision = ray.GetPoint(enter);

                float rightDistance = Vector3.Project(collision - origin, passThroughB - origin).magnitude;
                float upDistance = Vector3.Project(collision - origin, passThroughC - origin).magnitude;

                float worldWidth = (cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth,0, cam.nearClipPlane)) - origin).magnitude;
                float worldHeight = (cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane)) - origin).magnitude;

                float pixelWidth = (rightDistance / worldWidth) * cam.pixelWidth;
                float pixelHeight = (upDistance / worldHeight) * cam.pixelHeight;

                // Add vertex to mask
            }
        }
        */

        Texture2D mask = new Texture2D(cam.pixelWidth, cam.pixelHeight);
        mask.SetPixels(25, 25, 100, 100, new Color[]{ Color.white }, 16);

        mat.SetTexture("_Mask", newTexture);
        Graphics.Blit(source, destination, mat);
    }
}
