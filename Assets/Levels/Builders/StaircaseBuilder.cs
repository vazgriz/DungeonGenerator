using Assets.Level.Models;
using Assets.World.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class StaircaseBuilder : MonoBehaviour
    {
        [SerializeField]
        GameObject _staircase;

        public void BuildMany(ICollection<Staircase> staircases)
        {
            foreach (var staircase in staircases)
            {
                Build(staircase);
            }
        }

        public void Build(Staircase staircase)
        {
            // Goes Down->Up in North direction
            /*
             * north: (0.5, 0, 0) rotate 180 about y
                east: (0, 0, 0.5) rotate 270 about y
                south: (0.5, 0, 1) rotate 0 about y
                west: (1, 0, 0.5) rotate 90 about y
             * 
             * If staircase is going down in the given compass direction, the prefab needs to be
             *   - rotated 180 about y
             *   - moved 2f in given compass direction to offset movement from non centered rotation
             *   - lowered (-y)
             */

            var location = staircase.Previous + staircase.HorizontalOffset;
            var placedStaircase = Instantiate(_staircase, location, Quaternion.identity);

            int localRotation;
            float xOffset;
            float yOffset;
            float zOffset;

            switch (staircase.Direction)
            {
                case Direction.North:
                    localRotation = 180 + (staircase.IsUp ? 0 : 180);
                    xOffset = 0.5f;
                    yOffset = 0 + (staircase.IsUp ? 0 : -1f);
                    zOffset = 0 + (staircase.IsUp ? 0 : 2f);
                    break;
                case Direction.East:
                    localRotation = 270 + (staircase.IsUp ? 0 : 180);
                    xOffset = 0 + (staircase.IsUp ? 0 : 2f);
                    yOffset = 0 + (staircase.IsUp ? 0 : -1f);
                    zOffset = 0.5f;
                    break;
                case Direction.South:
                    localRotation = 0 + (staircase.IsUp ? 0 : 180);
                    xOffset = 0.5f;
                    yOffset = 0 + (staircase.IsUp ? 0 : -1f);
                    zOffset = 1 + (staircase.IsUp ? 0 : -2f);
                    break;
                case Direction.West:
                    localRotation = 90 + (staircase.IsUp ? 0 : 180);
                    xOffset = 1 + (staircase.IsUp ? 0 : -2f);
                    yOffset = 0 + (staircase.IsUp ? 0 : -1f);
                    zOffset = 0.5f;
                    break;
                default:
                    throw new ArgumentException();
            }

            placedStaircase.transform.localRotation = Quaternion.Euler(0, localRotation, 0);
            placedStaircase.transform.position += new Vector3(xOffset, yOffset, zOffset);
        }
    }
}