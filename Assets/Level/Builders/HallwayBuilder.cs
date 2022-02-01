using Assets.Helpers;
using Assets.Level.Models;
using Assets.World.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class HallwayBuilder : MonoBehaviour
    {
        [SerializeField]
        Material _hallwayMaterial;

        [SerializeField]
        GameObject _hallway; // Opens West/East

        [SerializeField]
        GameObject _hallwayL; // Opens South/East

        [SerializeField]
        GameObject _hallwayEnd; // Opens East

        public GameObject cubePrefab;

        private StructureBuilder _structureBuilder;

        public void Awake()
        {
            _structureBuilder = FindObjectOfType<StructureBuilder>();
        }

        public void BuildMany(ICollection<Hallway> hallways)
        {
            foreach (var hallway in hallways)
            {
                Build(hallway);
            }
        }    

        public void Build(Hallway hallway)
        {
            foreach (var piece in hallway.Pieces)
            {
                PlacePiece(piece.Location, piece.Previous, piece.Next);
            }
        }

        private void PlacePiece(Vector3Int location, LevelComponentPiece previous, Vector3Int? next)
        {
            _structureBuilder.BuildFloor(location.x, location.y, location.z);
            _structureBuilder.BuildCeiling(location.x, location.y + 1, location.z);

            var wallDirections = new List<Direction>();
            var (previousDirection, nextDirection) = GetNeighbourDirections(location, previous?.Location, next);

            foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
            {
                if ((previousDirection != null && direction == previousDirection)
                     || (nextDirection != null && direction == nextDirection))
                {
                    continue;
                }

                wallDirections.Add(direction);
            }

            foreach (var wallDirection in wallDirections)
            {
                _structureBuilder.BuildWall(location.x, location.y, location.z, wallDirection);
            }
        }

        private (Direction?, Direction?) GetNeighbourDirections(Vector3Int location, Vector3Int? previous, Vector3Int? next)
        {
            // previousDelta is the delta FROM current TO previous
            var previousDelta = previous != null ? previous.Value - location : (Vector3Int?)null;
            var nextDelta = next != null ? next.Value - location : (Vector3Int?)null;

            var previousDirection = GetDirectionForDelta(previousDelta);
            var nextDirection = GetDirectionForDelta(nextDelta);
            return (previousDirection, nextDirection);

            static Direction? GetDirectionForDelta(Vector3Int? delta)
            {
                if (delta == null)
                {
                    return null;
                }
                else if (delta.Value.x < 0)
                {
                    return Direction.West;
                }
                else if (delta.Value.x > 0)
                {
                    return Direction.East;
                }
                else
                {
                    if (delta.Value.z < 0)
                    {
                        return Direction.South;
                    }
                    else if (delta.Value.z > 0)
                    {
                        return Direction.North;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}