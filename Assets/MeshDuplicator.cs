using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDuplicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var hiddenLayer = GetLayer(HIDDEN_WINDOW_LAYER);
        foreach(var gameObject in FindObjectsOfType(typeof(GameObject)) as IEnumerable<GameObject>)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var layer = gameObject.layer;
            if(meshRenderer != null && layer != hiddenLayer)
            {
                var hiddenMeshLayer = new GameObject(gameObject.name + "(Hidden)");
                hiddenMeshLayer.layer = hiddenLayer;
                var hiddenMeshRenderer = hiddenMeshLayer.AddComponent<MeshRenderer>();
                var hiddenMeshFilter = hiddenMeshLayer.AddComponent<MeshFilter>();
                hiddenMeshFilter.mesh = gameObject.GetComponent<MeshFilter>().mesh;
                var newMeshName = meshRenderer.material.name+"hidden";

                hiddenMeshLayer.transform.parent = gameObject.transform;
            }

        }
    }

    private const string HIDDEN_WINDOW_LAYER = "HiddenWindow";
    private static int GetLayer(string layer)
    {
        switch (layer)
        {
            case HIDDEN_WINDOW_LAYER:
                return 9;
            default:
                return -1;
        }
    }
}
