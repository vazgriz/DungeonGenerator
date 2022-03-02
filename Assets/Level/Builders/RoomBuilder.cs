using Assets.Level.Models;
using Assets.World.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class RoomBuilder : MonoBehaviour
    {
        [SerializeField]

        private StructureBuilder _structureBuilder;

        public void Awake()
        {
            _structureBuilder = FindObjectOfType<StructureBuilder>();
        }

        public void BuildMany(ICollection<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Build(room);
            }
        }

        public void Build(Room room)
        {
            Build(room.Bounds.position, room.Bounds.size);
        }

        private void Build(Vector3Int location, Vector3Int size)
        {
            // TODO: Don't place prefab at entry point for room
            var width = size.x;
            var height = size.y;
            var depth = size.z;

            var xOrigin = location.x;
            var zOrigin = location.z;
            var yOrigin = location.y;

            for (var x = xOrigin; x - xOrigin < width; x++)
            {
                for (var z = zOrigin; z - zOrigin < depth; z++)
                {
                    _structureBuilder.BuildFloor(x, yOrigin, z);
                    _structureBuilder.BuildCeiling(x, yOrigin + height, z);                    
                }
            }

            _structureBuilder.BuildWalls(xOrigin, yOrigin, zOrigin, width, height, depth);
        }
    }
}