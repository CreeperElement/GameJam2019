using Assets.Camera;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCutoutRender : MonoBehaviour
{
    public Material windowMaterial;
    public DoorController controller;
    public Texture2D newTexture;

    private Camera cam;
    private Texture2D mask;

    private Color[] fillColorArray, pixelArray;
    private Plane clippingPlane;

    private Mesh windowMesh;
    GameObject CameraMesh;

	private void Start()
	{
        cam = GetComponent<Camera>();
        windowMesh = new Mesh();
        GenerateWindowMesh();
	}


    private void GenerateWindowMesh()
    {
        windowMesh.Clear();
        var cameraViewCorners = new Vector3[4];
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.nearClipPlane*1.00001f, Camera.MonoOrStereoscopicEye.Mono, cameraViewCorners);
        windowMesh.vertices = cameraViewCorners;
        var triangles = new int[] { 0, 1, 2, 2, 3, 0 };
        windowMesh.triangles = triangles;
        windowMesh.RecalculateNormals();
        windowMesh.Optimize();
        windowMesh.name = "WindowMesh";

        if (CameraMesh == null)
            CameraMesh = new GameObject("FramingMesh");
        if (CameraMesh.gameObject.GetComponent<Mesh>() == null)
            CameraMesh.gameObject.AddComponent<MeshFilter>();
        if (CameraMesh.gameObject.GetComponent<MeshRenderer>() == null)
            CameraMesh.gameObject.AddComponent<MeshRenderer>();
        CameraMesh.transform.position = gameObject.transform.position + gameObject.transform.forward* .0001f;
        CameraMesh.transform.eulerAngles = gameObject.transform.rotation.eulerAngles;
        CameraMesh.gameObject.GetComponent<MeshFilter>().mesh = windowMesh;
        CameraMesh.gameObject.GetComponent<Renderer>().material = windowMaterial;

        CameraMesh.gameObject.transform.parent = transform;
    }
}
