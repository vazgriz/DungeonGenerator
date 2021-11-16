using UnityEngine;

namespace Assets.Level.Builders
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField]
        Material greenMaterial;
        string stairsTag = "Stairs";

        public GameObject cubePrefab;

        private RoomBuilder _roomBuilder;
        private HallwayBuilder _hallwayBuilder;

        private void Awake()
        {
            _roomBuilder = FindObjectOfType<RoomBuilder>();
            _hallwayBuilder = FindObjectOfType<HallwayBuilder>();

        }
        public void Build(Models.Level level)
        {
            _roomBuilder.BuildMany(level.Rooms);
            _hallwayBuilder.BuildMany(level.Hallways);
        }

        public void PlaceStairs(Vector3Int location)
        {
            PlaceCube(location, new Vector3Int(1, 1, 1), greenMaterial, stairsTag);
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