using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssetGenerator : MonoBehaviour
{

    void Start()
    {
        // Search for and add all GameObjects (cubes) marked with a scene tag to an array
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        GameObject[] halls = GameObject.FindGameObjectsWithTag("Halls");
        GameObject[] stairs = GameObject.FindGameObjectsWithTag("Stairs");
    }

    void Update()
    {
        
    }
}
