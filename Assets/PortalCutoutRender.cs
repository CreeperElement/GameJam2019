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

    private Color[] fillColorArray;
    private Plane clippingPlane;

    private Mesh mesh;

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

        mat.SetTexture("_Mask", (Texture)mask);
        Debug.Log($"(Mask) Height: {mask.height} Width: {mask.width}");
        Debug.Log($"(newTexture) Height: {newTexture.height} Width: {newTexture.width}");

		mesh = controller.forwardPortal.GetComponent<MeshFilter>().mesh;
		//mat.SetTexture("_Mask", (Texture)newTexture);
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

        mask.SetPixels(fillColorArray);
		const int _RADIUS = 5;

		foreach (Vector2 point in vertices)
		{
			for (int x = 0; x < 2 * _RADIUS; x++)
			{
				for (int y = 0; y < 2 * _RADIUS; y++)
				{
					//Debug.Log(GetRadius(point, new Vector2(x, y)));
					if (GetRadius(point, new Vector2((int)point.x - _RADIUS + x, (int)point.y - _RADIUS + y)) <= _RADIUS)
					{
						//Debug.Log(point);
						mask.SetPixel((int)point.x - _RADIUS + x, (int)point.y - _RADIUS + y, Color.red);
					}
					else
					{
						mask.SetPixel((int)point.x - _RADIUS + x, (int)point.y - _RADIUS + y, new Color(0, 0, 0, 0));
					}
				}
			}
		}
		mask.Apply();

        //System.IO.File.WriteAllBytes(Application.dataPath + "/" + "picture5886.png", mask.EncodeToPNG
        mat.SetTexture("_Mask", (Texture)mask);
        Graphics.Blit(source, destination, mat);
    }

    private static float GetRadius(Vector2 center, Vector2 position)
    {
	    return Vector2.Distance(center, position);
    }

}
