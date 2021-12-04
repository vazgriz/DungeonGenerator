using Assets.World.Models;
using System;
using UnityEngine;

namespace Assets.Helpers
{
    public static class DirectionHelper
    {
        public static float GetRotationAboutYAxis(Direction direction)
        {
            // By default prefab faces East
            switch (direction)
            {
                case Direction.North:
                    //return Quaternion.AngleAxis(-90, new Vector3(0, 1));
                    //return Quaternion.Euler(new Vector3(0, -90, 0));
                    return -90;
                case Direction.East:
                    //return Quaternion.identity;
                    return 0;
                case Direction.South:
                    //return Quaternion.AngleAxis(90, new Vector3(0, 1));
                    return 90;
                case Direction.West:
                    //return Quaternion.AngleAxis(180, new Vector3(0, 1));
                    return 180;
                default:
                    throw new ArgumentException("Invalid Direction when building Hallways");
            }
        }

        public static void RotateAboutYAxis(GameObject gameObject, Direction direction)
        {
            var rotation = GetRotationAboutYAxis(direction);
            gameObject.transform.RotateAround(gameObject.GetComponent<Renderer>().bounds.center, new Vector3(0, 1, 0), rotation);
        }
    }
}