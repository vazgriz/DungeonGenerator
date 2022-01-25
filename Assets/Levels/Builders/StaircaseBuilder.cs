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
             */
            var location = staircase.Previous + staircase.HorizontalOffset;

            var placedStaircase = Instantiate(_staircase, location, Quaternion.identity);

            switch(staircase.Direction)
            {
                case Direction.North:
                    placedStaircase.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    placedStaircase.transform.position += new Vector3(0.5f, 0, 0);
                    break;
                case Direction.East:
                    placedStaircase.transform.localRotation = Quaternion.Euler(0, 270, 0);
                    placedStaircase.transform.position += new Vector3(0, 0, 0.5f);
                    break;
                case Direction.South:
                    placedStaircase.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    placedStaircase.transform.position += new Vector3(0.5f, 0, 1);
                    break;
                case Direction.West:
                    placedStaircase.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    placedStaircase.transform.position += new Vector3(1, 0, 0.5f);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}