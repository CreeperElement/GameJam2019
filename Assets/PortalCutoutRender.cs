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
        // Puts a new 4 vertex mesh into the game object called CameraMesh
        RefreshCameraMesh();
        var mesh = CameraMesh.GetComponent<Mesh>();


    }

    private GameObject RefreshCameraMesh()
    {
        coverMesh.Clear();
        var cameraViewCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane * 1.00001f, Camera.MonoOrStereoscopicEye.Mono, cameraViewCorners);
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
        CameraMesh.transform.position = gameObject.transform.position + gameObject.transform.forward * .0001f;
        CameraMesh.transform.eulerAngles = gameObject.transform.rotation.eulerAngles;
        CameraMesh.gameObject.GetComponent<MeshFilter>().mesh = coverMesh;

        GetVertices();
        return CameraMesh;
    }

    private void GetVertices()
    {
        /* Step 1: We make a plane. Unity allows us to define a plane by defining three points which the plane
         * is allowed to pass through. Theoretically, if we use the forward, up, and right components of the camera
         * in combination with the nearClipPlane distance; we can create three points on the nearClipPlane, and 
         * define the nearClipPlane.
         */

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

        // Get the shape
        var lines = getShape(vertices);
        // Fill in the shape

        var allLineVertices = new List<Vector2>();
        // I think this will work to make all combinations
        // ABCDE => AB, AC, AD, AE, BC, BD, BE, CD, CE, DE
        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = i; j < lines.Count; j++)
            {
                allLineVertices.AddRange(GetInBoundsIntersection(lines[i], lines[j], lines));
            }
        }
        var corners = ConvertScreenToMeshPoints();
        var meshPoint = ConvertVerticesToMeshPoints(allLineVertices);

    }

    private Vector3[] ConvertScreenToMeshPoints() {
        var cameraViewCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane * 1.00001f, Camera.MonoOrStereoscopicEye.Mono, cameraViewCorners);
        var BottomLeft = cameraViewCorners[0];
        var BottomRight = cameraViewCorners[3];
        var TopLeft = cameraViewCorners[1];

        var Right = BottomRight - BottomLeft;
        var Up = TopLeft - BottomLeft;

        return new Vector3[] { TopLeft, BottomRight + Up, BottomLeft, BottomRight };
    }

    private static Vector3[] ConvertVerticesToMeshPoints(List<Vector2> points)
    {
        
    }

    private static List<Vector2> GetInBoundsIntersection(Line a, Line b, List<Boundary> boundaries)
    {
        var points = new List<Vector2>();
        var intersection = a.Intersection(b);
        if (inBounds(intersection, boundaries))
            points.Add(intersection);
        return points;
    }

    private static bool inBounds(Vector2 point, List<Boundary> boundaries)
    {
        foreach(var boundary in boundaries)
        {
            if (!boundary.inBounds(point))
                return false;
        }
        return true;
    }

    private static float GetRadius(Vector2 center, Vector2 position)
    {
	    return Vector2.Distance(center, position);
    }

    private static bool inBounds(int x, int y, List<Boundary> bounds)
    {
        foreach(var boundary in bounds)
        {
            if (!boundary.inBounds(new Vector2(x, y))) return false;
        }
        return true;
    }

    private static List<Boundary> getShape(Vector2[] vertices)
    {
        // The vertices come from a mesh, which must have at least three vertices
        var bounds = new List<Boundary>();
        var workingLine = new Line(vertices[0], vertices[1]);
        var boundaryCondition = workingLine.pointIsAbove(vertices[2]) ?
            BoundaryCondition.Above : BoundaryCondition.Below;
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[0], vertices[1]), vertices[2]));
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[1], vertices[2]), vertices[0]));
        bounds.Add(getBoundaryIncludingPoint(new Line(vertices[2], vertices[0]), vertices[1]));

        List<Vector2> outOfBoundsVectors = new List<Vector2>();
        foreach (Vector2 point in vertices)
        {
            foreach (Boundary bound in bounds)
            {
                if (!bound.inBounds(point))
                {
                    outOfBoundsVectors.Add(point);
                    break;
                }
            }
        }

        // Find the points caught in the corners
        for (int i = 0; i < bounds.Count; i++)
        {
            var firstBound = bounds[i];
            var secondBound = bounds[(i + 1) % bounds.Count];

            var collision = firstBound.Intersection(secondBound);
            var firstBoundInverse = -1 / firstBound.Slope;
            var secondBoundInverse = -1 / secondBound.Slope;

            var firstBoundPerpendicular = new Boundary(collision, new Vector2(collision.x + 1, collision.y + firstBoundInverse), firstBound.boundary);
            var secondBoundPerpendicular = new Boundary(collision, new Vector2(collision.x + 1, collision.y + secondBoundInverse), secondBound.boundary);

            foreach (var vertex in vertices)
            {
                // InvertedInBounds might not necessarily be the right choice but I can't prove otherwise
                if(firstBoundPerpendicular.invertedInBounds(vertex) && secondBoundPerpendicular.invertedInBounds(vertex))
                {
                    var firstBoudIntersection = firstBound.Intersection(bounds[(bounds.Count - (i+1))]);
                    var secondBoundIntersection = secondBound.Intersection(bounds[(i + 2) % bounds.Count]);
                    bounds[i] = new Boundary(firstBoudIntersection, vertex, firstBound.boundary);
                    bounds[(i + 1) % bounds.Count] = new Boundary(secondBoundIntersection, vertex, secondBound.boundary);
                }
            }
        }

        // Assumption: For the sake of efficiency we are going to keep the lines in order so that the two 
        // endpoints of the line intersect with the lines before and after it in the list
        bounds = RecursiveGetShape(vertices, bounds);

        return bounds;
    }

    private static List<Boundary> RecursiveGetShape(Vector2[] vertices, List<Boundary> bounds)
    {
        if (vertices.Length == 0)
            return bounds;

        List<Vector2> outOfBoundsVectors = new List<Vector2>();
        foreach(Vector2 point in vertices)
        {
            foreach (Boundary bound in bounds) {
                if(!bound.inBounds(point))
                {
                    outOfBoundsVectors.Add(point);
                    break;
                }
            }
        }

        Vector2[] points = outOfBoundsVectors.ToArray();
        return RecursiveGetShape(points, bounds);
    }

    private static Boundary getBoundaryIncludingPoint(Line line, Vector2 point)
    {
        var boundaryCondition = line.pointIsAbove(point) ?
            BoundaryCondition.Above : BoundaryCondition.Below;
        return new Boundary(line, boundaryCondition);
    }

}
