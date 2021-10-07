using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssetGenerator : MonoBehaviour
{

    [SerializeField]
    Material redMaterial;
    string roomTag = "Room";
    [SerializeField]
    Material blueMaterial;
    string hallTag = "Hall";
    [SerializeField]
    Material greenMaterial;
    string stairsTag = "Stairs";

    [SerializeField]
    GameObject floorPrefab;
    [SerializeField]
    GameObject corridorPrefab;
    [SerializeField]
    GameObject stairsPrefab;

    // Modular Prefabs For Scene
    // Floors
    [SerializeField]
    GameObject centre;

    public GameObject cubePrefab;

    void Start()
    {
    }

    void Update()
    {   
    }

    bool placed;

    public void PlaceRoom(Vector3Int location, Vector3Int size)
    {
        //if (placed)
        //{
        //    return;
        //}

        //placed = true;

        Debug.Log($"Size: {size.x}, {size.y}, {size.z}");
        Debug.Log($"Location: {location.x}, {location.y}, {location.z}");

        for (var x = location.x; x <= location.x + size.x; x++)
        {
            for (var z = location.z; z <= location.z + size.z; z++)
            {
                Instantiate(centre,new Vector3Int(x, location.y, z), Quaternion.identity);
            }
        }
    }

    public void PlaceHallway(Vector3Int location)
    {
        PlaceCube(location, new Vector3Int(1, 1, 1), blueMaterial, hallTag);
    }
    public void PlaceStairs(Vector3Int location)
    {
        PlaceCube(location, new Vector3Int(1, 1, 1), greenMaterial, stairsTag);
    }
    private void PlaceCube(Vector3Int location, Vector3Int size, Material material, string tag)
    {
        GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = size;
        go.GetComponent<MeshRenderer>().material = material;
        go.tag = tag;
    }
}
