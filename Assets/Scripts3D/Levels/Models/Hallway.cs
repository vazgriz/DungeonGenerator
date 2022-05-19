using UnityEngine;

namespace Assets.Level.Models
{
    public class Hallway : LevelComponent
    {
        public Hallway(Vector3Int location, Vector3Int size) : base(location, size)
        {
        }

        public Hallway() : base()
        {
        }
    }
}