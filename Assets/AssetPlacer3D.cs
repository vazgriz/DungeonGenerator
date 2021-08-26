using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPlacer3D : MonoBehaviour
{
    void Start()
    {

    GameObject[] arrayofcubes = GameObject.FindGameObjectsWithTag("Cube");  

    Debug.Log(arrayofcubes.Length);

    }
}
