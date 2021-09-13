using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssetGenerator : MonoBehaviour
{

    List<GameObject> placeHolderCubes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        placeHolderCubes.Add(GameObject.FindGameObjectWithTag("Stairs"));
        placeHolderCubes.Add(GameObject.FindGameObjectWithTag("Room"));
        placeHolderCubes.Add(GameObject.FindGameObjectWithTag("Hall"));

        Debug.Log(placeHolderCubes.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
