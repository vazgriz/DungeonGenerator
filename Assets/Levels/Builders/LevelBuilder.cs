using UnityEngine;

namespace Assets.Level.Builders
{
    public class LevelBuilder : MonoBehaviour
    {
        private RoomBuilder _roomBuilder;
        private HallwayBuilder _hallwayBuilder;
        private StaircaseBuilder _staircaseBuilder;

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
        }
    }
}