using Assets.World.Models;
using System;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class StructureBuilder : MonoBehaviour
    {
        [SerializeField]
        GameObject _ceiling;
        [SerializeField]
        GameObject _floor;
        [SerializeField]
        GameObject _wall;

        public void BuildFloor(int x, int y, int z)
        {
            Instantiate(_floor, new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.identity);
        }

        public void BuildCeiling(int x, int y, int z)
        {
            Instantiate(_ceiling, new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.identity);
        }

        public void BuildWalls(int xOrigin, int yOrigin, int zOrigin, int width, int height, int depth)
        {
            // Build North/South wall
            for (var x = xOrigin; x < xOrigin + width; x++)
            {
                for (var y = yOrigin; y < yOrigin + height; y++)
                {
                    BuildWall(x, y, zOrigin, Direction.South);
                    BuildWall(x, y, zOrigin + depth - 1, Direction.North);
                }
            }

            // Build East/West wall
            for (var z = zOrigin; z < zOrigin + depth; z++)
            {
                for (var y = yOrigin; y < yOrigin + height; y++)
                {
                    BuildWall(xOrigin, y, z, Direction.West);
                    BuildWall(xOrigin + width - 1, y, z, Direction.East);
                }
            }
        }

        public void BuildWall(int x, int y, int z, Direction direction)
        {
            // Wall by default sits on North side of origin unit square
            switch (direction)
            {
                case Direction.North: 
                    var northWall = Instantiate(_wall, new Vector3(x + 0.5f, y, z + 1), Quaternion.identity);
                    northWall.transform.localRotation = Quaternion.identity;
                    break;
                case Direction.East:
                    var eastWall = Instantiate(_wall, new Vector3(x + 1, y, z + 0.5f), Quaternion.identity);
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