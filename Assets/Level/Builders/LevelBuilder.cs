using UnityEngine;

namespace Assets.Level.Builders
{
    public class LevelBuilder : MonoBehaviour
    {
        [SerializeField]
        Material greenMaterial;
        string stairsTag = "Stairs";

        [SerializeField]
        Material redMaterial;
        string roomTag = "Room";
        [SerializeField]
        Material blueMaterial;
        string hallTag = "Hall";

        public GameObject cubePrefab;

        private RoomBuilder _roomBuilder;
        private HallwayBuilder _hallwayBuilder;
        private StaircaseBuilder _staircaseBuilder;
        bool havePlaced = false;

        private void Awake()
        {
            _roomBuilder = FindObjectOfType<RoomBuilder>();
            _hallwayBuilder = FindObjectOfType<HallwayBuilder>();
            _staircaseBuilder = FindObjectOfType<StaircaseBuilder>();
        }

        public void Build(Models.Level level)
        {
            _roomBuilder.BuildMany(level.Rooms);
            _hallwayBuilder.BuildMany(level.Hallways);
            _staircaseBuilder.BuildMany(level.Staircases);

            Debug.Log($"***** LEVEL BUILT *****");
        }

        public void PlaceStairs(Vector3Int location)
        {
            //PlaceCube(location, new Vector3Int(1, 1, 1), greenMaterial, stairsTag);
        }

        public void PlaceStairSet(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset)
        {
            //return;
            //if (havePlaced)
            //{
            //    return;
            //}
            //havePlaced = true;

            //Debug.Log($"Placing stairs with args: {previous}, {verticalOffset}, {horizontalOffset}");
            //Debug.Log($"{previous + horizontalOffset}");
            //Debug.Log($"{previous + horizontalOffset * 2}");
            //Debug.Log($"{previous + verticalOffset + horizontalOffset}");
            //Debug.Log($"{previous + verticalOffset + horizontalOffset * 2}");


            PlaceStairs(previous + horizontalOffset);
            PlaceStairs(previous + horizontalOffset * 2);
            PlaceStairs(previous + verticalOffset + horizontalOffset);
            PlaceStairs(previous + verticalOffset + horizontalOffset * 2);
        }

        public void PlaceRoom(Vector3Int location, Vector3Int size)
        {
            //PlaceCube(location, size, redMaterial, roomTag);
        }
        public void PlaceHallway(Vector3Int location)
        {
            PlaceCube(location, new Vector3Int(1, 1, 1), blueMaterial, hallTag);
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