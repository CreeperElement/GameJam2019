using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
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
                var newMeshName = RemoveText(meshRenderer.material.name, " (Instance)");
                var materialPaths = AssetDatabase.FindAssets(newMeshName);
                string materialPath = "Assets\\Materials\\Resources\\DefaultMaterial_Hidden.mat";
                foreach(var path in materialPaths)
                {
                    var rootPath = RemoveText(AssetDatabase.GUIDToAssetPath(path), newMeshName + ".mat");
                    // At this point only an exact match will end with a slash
                    //Path/RedMaterial.mat will simply be Path/ if we searched redmaerial
                    //Path/RedMaterial_Gloves.mat will not match
                    if (rootPath.EndsWith("/"))
                    {
                        materialPath = rootPath + newMeshName + "_Hidden.mat";
                        break;
                    }
                }

                var loadedMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
                if (loadedMaterial == null)
                {
                    loadedMaterial = AssetDatabase.LoadAssetAtPath("Assets\\Materials\\Resources\\DefaultMaterial_Hidden.mat", typeof(Material)) as Material;
                    Debug.Log("Null");
                }
                hiddenMeshRenderer.material = loadedMaterial;
                Debug.Log(loadedMaterial + " : "+ hiddenMeshRenderer.material);
                hiddenMeshLayer.transform.position = gameObject.transform.position;
                hiddenMeshLayer.transform.localEulerAngles = gameObject.transform.localEulerAngles;
                hiddenMeshLayer.transform.parent = gameObject.transform;
            }

        }
    }
    /// <summary>
    /// Removes one string from another. The original string is returned if it doesn't contain the other string
    /// </summary>
    /// <param name="original">The string we want to remove text from</param>
    /// <param name="textToRemove">The subset of the larger string to remove.</param>
    /// <returns>The original text wiht the string removed. Returns the original if it doesn't contain the subset.</returns>
    private static string RemoveText(string original, string textToRemove)
    {
        if (!original.Contains(textToRemove))
            return original;

        var newString = (string)original.Clone();
        while(newString.Contains(textToRemove))
        {
            var index = newString.IndexOf(textToRemove);
            var endIndex = index + textToRemove.Length;
            newString = newString.Substring(0, index) + newString.Substring(endIndex, newString.Length - endIndex);
        }

        return newString;
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
