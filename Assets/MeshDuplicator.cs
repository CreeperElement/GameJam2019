using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
                Debug.Log(newMeshName);

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

        var originalArray = Regex.Split(original, textToRemove);
        Debug.Log(originalArray.Length);
        var arrayLength = originalArray.Length;
        var i = 0;
        var newString = "";

        do
        {
            if(!originalArray[i].Equals(textToRemove))
                newString += originalArray[i];
            i++;
        } while (i < arrayLength);

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
