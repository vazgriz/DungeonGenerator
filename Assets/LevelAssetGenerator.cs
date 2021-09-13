using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssetGenerator : MonoBehaviour
{

    List<GameObject> placeHolderCubes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        placeHolderCubes.Add(GameObject.Find("cube(Clone)"));

        Debug.Log(placeHolderCubes.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
