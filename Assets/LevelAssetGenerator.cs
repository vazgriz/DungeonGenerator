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
    [SerializeField]
    GameObject southWestCorner;
    [SerializeField]
    GameObject southEastCorner;
    [SerializeField]
    GameObject northWestCorner;
    [SerializeField]
    GameObject northEastCorner;

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
        Debug.Log($"Size: {size.x}, {size.y}, {size.z}");
        Debug.Log($"Location: {location.x}, {location.y}, {location.z}");

        var width = location.x + size.x;
        var depth = location.z + size.z;
        var xOrigin = location.x;
        var zOrigin = location.z;

        for (var x = xOrigin; x <= width; x++)
        {
            for (var z = zOrigin; z <= depth; z++)
            {
                var modularPrefab = GetModularPrefab(xOrigin, zOrigin, x, z, width, depth);

                Instantiate(modularPrefab, new Vector3Int(x, location.y, z), Quaternion.identity);
                
            }
        }
    }

    private GameObject GetModularPrefab(int minX, int minZ, int x, int z, int maxX, int maxZ)
    {
        if (z == minZ)
        {
            if (x == minX)
            {
                return southWestCorner;
            }
            else if (x == maxX)
            {
                return southEastCorner;
            }
            else
            {
                // insert south z edge piece
            }
        }
        else if (z == maxZ)
        {
            if (x == minX)
            {
                return northWestCorner;
            }
            else if (x == maxX)
            {
                return northEastCorner;
            }
            else
            {
                // insert south z edge piece
            }
        }
        return centre;
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
