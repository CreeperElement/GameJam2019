using Assets.Camera;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCutoutRender : MonoBehaviour
{
    public Material mat;
    public Shader spriteShader;
    public DoorController controller;
    public Texture2D newTexture;

    private Camera cam;
    private Texture2D mask;

    private Color[] fillColorArray, pixelArray;
    private Plane clippingPlane;

    private Mesh mesh, coverMesh;
    GameObject CameraMesh;

	private void Start()
	{
		cam = GetComponent<Camera>();
		mask = new Texture2D(cam.pixelWidth, cam.pixelHeight, TextureFormat.ARGB32, true);

		fillColorArray = mask.GetPixels();
		for (int i = 0; i < fillColorArray.Length; i++)
		{
			fillColorArray[i] = Color.black;
		}
		mask.SetPixels(fillColorArray);
        mask.Apply();

        pixelArray = new Color[fillColorArray.Length];
        fillColorArray.CopyTo(pixelArray, 0);

        mat.SetTexture("_Mask", (Texture)mask);
        Debug.Log($"(Mask) Height: {mask.height} Width: {mask.width}");
        Debug.Log($"(newTexture) Height: {newTexture.height} Width: {newTexture.width}");

		mesh = controller.forwardPortal.GetComponent<MeshFilter>().mesh;
        coverMesh = new Mesh();
		//mat.SetTexture("_Mask", (Texture)newTexture);
	}


    private void OnPreRender()
    {
        coverMesh.Clear();
        var cameraViewCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane*1.00001f, Camera.MonoOrStereoscopicEye.Mono, cameraViewCorners);
        coverMesh.vertices = cameraViewCorners;
        var triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        coverMesh.triangles = triangles;
        coverMesh.RecalculateNormals();
        coverMesh.Optimize();
        coverMesh.name = "FramingMesh";

        if (CameraMesh == null)
            CameraMesh = new GameObject("FramingMesh");
        if (CameraMesh.gameObject.GetComponent<Mesh>() == null)
            CameraMesh.gameObject.AddComponent<MeshFilter>();
        if (CameraMesh.gameObject.GetComponent<MeshRenderer>() == null)
            CameraMesh.gameObject.AddComponent<MeshRenderer>();
        CameraMesh.transform.position = gameObject.transform.position + gameObject.transform.forward* .0001f;
        CameraMesh.transform.eulerAngles = gameObject.transform.rotation.eulerAngles;
        CameraMesh.gameObject.GetComponent<MeshFilter>().mesh = coverMesh;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        /* Step 1: We make a plane. Unity allows us to define a plane by defining three points which the plane
         * is allowed to pass through. Theoretically, if we use the forward, up, and right components of the camera
         * in combination with the nearClipPlane distance; we can create three points on the nearClipPlane, and 
         * define the nearClipPlane.
         */
         
        Vector3 passThroughA, passThroughB, passThroughC;
        passThroughA = transform.position + transform.forward * cam.nearClipPlane;
        passThroughB = passThroughA + transform.right;
        passThroughC = passThroughA + transform.up;
        clippingPlane = new Plane(passThroughA, passThroughB, passThroughC);

        Transform portalTransform = controller.transform;

        Vector2[] vertices = new Vector2[mesh.vertices.Length];
        int vertexCount = 0;

		foreach (Vector3 vertex in mesh.vertices)
		{
			// Get the real world position of the vector
			Vector3 vertexPosition = portalTransform.TransformPoint(vertex);
			// Add the location of the vector to the screen
			vertices[vertexCount++] = cam.WorldToScreenPoint(vertexPosition);
		}

		const int _RADIUS = 5;

        // Get the shape
        var lines = getShape(vertices);
        // Fill in the shape

        var width = mask.width;
        var height = mask.height;

        // This section at the moment is unusable. It is too drastic a framerate drop to be used
        /*for(int y = 0; y < height; y++)
        {
            var lineOffset = y * width;
            for(int x = 0; x < width; x++)
            {
                if (inBounds(x, y, lines))
                    pixelArray[lineOffset + x] = Color.white;
                else
                    pixelArray[lineOffset + x] = new Color(0, 0, 0, 0);
            }
        }*/

        //mask.SetPixels(pixelArray);
        //mask.Apply();
        Debug.Log("hello");
        


        //System.IO.File.WriteAllBytes(Application.dataPath + "/" + "picture5886.png", mask.EncodeToPNG
        mat.SetTexture("_Mask", (Texture)mask);
        Graphics.Blit(source, destination, mat);
    }

    private static float GetRadius(Vector2 center, Vector2 position)
    {
	    return Vector2.Distance(center, position);
    }

    private bool inBounds(int x, int y, List<Boundary> bounds)
    {
        foreach(var boundary in bounds)
        {
            if (!boundary.inBounds(new Vector2(x, y))) return false;
        }
        return true;
    }

    private List<Boundary> getShape(Vector2[] vertices)
    {
        // The vertices come from a mesh, which must have at least three vertices
        var bounds = new List<Boundary>();
        var workingLine = new Line(vertices[0], vertices[1]);
        var boundaryCondition = workingLine.pointIsAbove(vertices[2]) ?
            BoundaryCondition.Above : BoundaryCondition.Below;
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[0], vertices[1]), vertices[2]));
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[1], vertices[2]), vertices[0]));
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[2], vertices[0]), vertices[1]));

        return bounds;
    }

    private Boundary getBoundaryIncludingPoint(Line line, Vector2 point)
    {
        var boundaryCondition = line.pointIsAbove(point) ?
            BoundaryCondition.Above : BoundaryCondition.Below;
        return new Boundary(line, boundaryCondition);
    }

}
