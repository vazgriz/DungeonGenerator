using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField]
    GameObject southEdge;
    [SerializeField]
    GameObject northEdge;
    [SerializeField]
    GameObject westEdge;
    [SerializeField]
    GameObject eastEdge;

    public GameObject cubePrefab;

    void Start()
    {
        StartCoroutine(PlaceCubes());
    }

    private IEnumerator PlaceCubes()
    {
        foreach (var placement in CubePlacements)
        {
            Debug.Log($"Starting, the time is {DateTime.Now} waiting 5 seconds...");
            yield return new WaitForSeconds(0);
            Debug.Log($"Placing cube at {DateTime.Now}");
            placement.Invoke(9);
        }
    }

    void Update()
    {   
    }

    List<Func<int, int>> CubePlacements = new List<Func<int, int>>();

    public void PlaceRoom(Vector3Int location, Vector3Int size)
    {
        //Debug.Log($"Size: {size.x}, {size.y}, {size.z}");
        //Debug.Log($"Location: {location.x}, {location.y}, {location.z}");

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
                return southEdge;
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
                return northEdge;
            }
        }
        else if (x == minX)
        {
            return westEdge;
        }
        else if (x == maxX)
        {
            return eastEdge;
        }

        return centre;
    }


    public void PlaceHallway(Vector3Int location, Vector3Int? previous, Vector3Int? next)
    {
        PlaceCube(location, new Vector3Int(1, 1, 1), blueMaterial, hallTag);
        //return 1;
        
    }

    public void PlaceStairs(Vector3Int location)
    {
        //CubePlacements.Add(_ =>
        {
            PlaceCube(location, new Vector3Int(1, 1, 1), greenMaterial, stairsTag);
            //return 1;
        };
    }

    public void PlaceStairSet(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset)
    {
        PlaceStairs(previous + horizontalOffset);
        PlaceStairs(previous + horizontalOffset * 2);
        PlaceStairs(previous + verticalOffset + horizontalOffset);
        PlaceStairs(previous + verticalOffset + horizontalOffset * 2);
    }

    private void PlaceCube(Vector3Int location, Vector3Int size, Material material, string tag)
    {
        //Debug.Log($"Placed {tag} at ({location.x}, {location.y}, {location.z})");
        GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
        go.GetComponent<Transform>().localScale = size;
        go.GetComponent<MeshRenderer>().material = material;
        go.tag = tag;
    }
}
