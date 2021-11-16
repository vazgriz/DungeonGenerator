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
            var (previousDirection, nextDirection) = GetNeighbourDirections(location, previous?.Location, next);
            var (modularPrefab, rotation) = GetOrientedPrefabForDirections(previousDirection, nextDirection);

            var piece = Instantiate(modularPrefab, location, rotation);
            piece.GetComponent<MeshRenderer>().material = _hallwayMaterial;
        }

        private (Direction?, Direction?) GetNeighbourDirections(Vector3Int location, Vector3Int? previous, Vector3Int? next)
        {
            var previousDelta = previous != null ? location - previous.Value : Vector3Int.zero;
            var nextDelta = next != null ? location - next : Vector3Int.zero;

            var previousDirection = GetDirectionForDelta(previousDelta);
            var nextDirection = GetDirectionForDelta(nextDelta);
            return (previousDirection, nextDirection);
            

            Direction? GetDirectionForDelta(Vector3Int? delta)
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

        private (GameObject, Quaternion) GetOrientedPrefabForDirections(Direction? previous, Direction? next)
        {
            if (previous == null && next == null)
            {
                throw new ArgumentException("Trying to place a Hallway piece with no neighbours");
            }

            if (previous == null || next == null)
            {
                var rotation = GetOrientationForSingleDirection(previous ?? next.Value);
                return (_hallwayEnd, rotation);
            }

            var directions = new List<Direction> { previous.Value, next.Value };

            if (directions.Contains(Direction.North) && directions.Contains(Direction.South))
            {
                return (_hallway, Quaternion.AngleAxis(90, new Vector3(0, 1)));
            }
            else if (directions.Contains(Direction.East) && directions.Contains(Direction.West))
            {
                return (_hallway, Quaternion.identity);
            }
            else if (directions.Contains(Direction.North) && directions.Contains(Direction.East))
            {
                return (_hallwayL, Quaternion.AngleAxis(-90, new Vector3(0, 1)));
            }
            else if (directions.Contains(Direction.East) && directions.Contains(Direction.South))
            {
                return (_hallwayL, Quaternion.identity);
            }
            else if (directions.Contains(Direction.South) && directions.Contains(Direction.West))
            {
                return (_hallwayL, Quaternion.AngleAxis(90, new Vector3(0, 1)));
            }
            else if (directions.Contains(Direction.West) && directions.Contains(Direction.North))
            {
                return (_hallwayL, Quaternion.AngleAxis(180, new Vector3(0, 1)));
            }

            throw new ArgumentException("Hallway pieces in wrong configuration");
        }

        private Quaternion GetOrientationForSingleDirection(Direction direction)
        {
            // By default prefab faces East
            switch(direction)
            {
                case Direction.North:
                    return Quaternion.AngleAxis(-90, new Vector3(0, 1));
                case Direction.East:
                    return Quaternion.identity;
                case Direction.South:
                    return Quaternion.AngleAxis(90, new Vector3(0, 1));
                case Direction.West:
                    return Quaternion.AngleAxis(180, new Vector3(0, 1));
                default:
                    throw new ArgumentException("Invalid Direction when building Hallways");
            }
                
        }
    }
}