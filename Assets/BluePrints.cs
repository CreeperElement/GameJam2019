using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluePrints : MonoBehaviour
{
    public static BluePrints Instance;

    public GameObject _CameraPrefab;
    public static GameObject CameraPrefab {
        get {
            return Instance._CameraPrefab;
        }
    }

    public int _MAX_RENDER_LAYERS;
    public static int MAX_RENDER_LAYERS {
        get {
            return Instance._MAX_RENDER_LAYERS;
        }
    }

    private void Awake()
    {
        Instance = this;
    }
}
