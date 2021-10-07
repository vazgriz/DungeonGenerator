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

    public GameObject cubePrefab;

    void Start()
    {
    }

    void Update()
    {   
    }

    public void PlaceRoom(Vector3Int location, Vector3Int size)
    {
        PlaceCube(location, size, redMaterial, roomTag);
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
