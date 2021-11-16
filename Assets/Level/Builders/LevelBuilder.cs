using UnityEngine;

namespace Assets.Level.Builders
{
    public class LevelBuilder : MonoBehaviour
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

        private RoomBuilder _roomBuilder;

        private void Awake()
        {
            _roomBuilder = FindObjectOfType<RoomBuilder>();

        }
        public void Build(Models.Level level)
        {

        }

        public void PlaceRoom(Vector3Int location, Vector3Int size)
        {
            _roomBuilder.Build(location, size);
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
}