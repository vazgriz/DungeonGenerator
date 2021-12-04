using Assets.Level.Models;
using Assets.World.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class RoomBuilder : MonoBehaviour
    {
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

        [SerializeField]
        GameObject _ceiling;
        [SerializeField]
        GameObject _floor;
        [SerializeField]
        GameObject _wall;


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
                    BuildFloor(x, yOrigin, z);
                    BuildCeiling(x, yOrigin + height, z);                    
                }
            }

            BuildWalls(xOrigin, yOrigin, zOrigin, width, height, depth);
        }

        private void BuildFloor(int x, int y, int z)
        {
            Instantiate(_floor, new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.identity);
        }

        private void BuildCeiling(int x, int y, int z)
        {
            Instantiate(_ceiling, new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.identity);
        }

        private void BuildWalls(int xOrigin, int yOrigin, int zOrigin, int width, int height, int depth)
        {
            // Build North/South wall
            for (var x = xOrigin; x < xOrigin + width; x++)
            {
                for (var y = yOrigin; y < yOrigin + height; y++)
                {
                    BuildWall(x, y, zOrigin, Direction.South);
                    BuildWall(x, y, zOrigin + depth, Direction.North);
                }
            }

            // Build East/West wall
            for (var z = zOrigin; z < zOrigin + depth; z++)
            {
                for (var y = yOrigin; y < yOrigin + height; y++)
                {
                    BuildWall(xOrigin, y, z, Direction.West);
                    BuildWall(xOrigin + width, y, z, Direction.East);
                }
            }
        }

        private void BuildWall(int x, int y, int z, Direction direction)
        {
            // Wall by default sits on North side of origin unit square
            switch (direction)
            {
                case Direction.North:
                    Instantiate(_wall, new Vector3(x + 0.5f, y, z), Quaternion.identity);
                    break;
                case Direction.East:
                    var eastWall = Instantiate(_wall, new Vector3(x, y, z + 0.5f), Quaternion.identity);
                    eastWall.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case Direction.South:
                    var southWall = Instantiate(_wall, new Vector3(x + 0.5f, y, z), Quaternion.identity);
                    southWall.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
                case Direction.West:
                    var westWall = Instantiate(_wall, new Vector3(x, y, z + 0.5f), Quaternion.identity);
                    westWall.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    break;
                default:
                    throw new ArgumentException("Invalid direction when building walls");
            }
        }
    }
}