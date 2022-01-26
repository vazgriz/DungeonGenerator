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
                        
            var piecesString = string.Empty;
            foreach (var piece in hallway.Pieces)
            {
                piecesString += LocationString(piece.Location);

                if (piece.Previous != null)
                {
                    piecesString += Spaced("Previous:");
                    piecesString += LocationString(piece.Previous.Location);
                }

                if (piece.Next != null)
                {
                    piecesString += Spaced("Next:");
                    piecesString += LocationString(piece.Next.Value);
                }
                piecesString += Environment.NewLine;
            }

            //Debug.Log($"Built Hallway: {hallway} with pieces: {piecesString}");



            string LocationString(Vector3Int location) => Spaced($"{location.x}") + Spaced($"{location.y}") + Spaced($"{location.z}") + Environment.NewLine;
            string Spaced(string input) => input + " ";
        }

        private void PlacePiece(Vector3Int location, LevelComponentPiece previous, Vector3Int? next)
        {
            var (previousDirection, nextDirection) = GetNeighbourDirections(location, previous?.Location, next);

            _structureBuilder.BuildFloor(location.x, location.y, location.z);
            _structureBuilder.BuildCeiling(location.x, location.y + 1, location.z);

            var wallDirections = new List<Direction>();

            foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
            {
                if ((previousDirection != null && direction == previousDirection)
                     || (nextDirection != null && direction == nextDirection))
                {
                    continue;
                }

                wallDirections.Add(direction);
            }

            //Debug.Log($"Piece with neighbours {previousDirection}, {nextDirection}. Placing walls: ");

            foreach (var wallDirection in wallDirections)
            {
                //Debug.Log($"{wallDirection}");
                _structureBuilder.BuildWall(location.x, location.y, location.z, wallDirection);
            }
        }

        private (Direction?, Direction?) GetNeighbourDirections(Vector3Int location, Vector3Int? previous, Vector3Int? next)
        {
            // previousDelta - the delta FROM curent TO previous
            var previousDelta = previous != null ? previous.Value - location : (Vector3Int?)null;
            var nextDelta = next != null ? next.Value - location : (Vector3Int?)null;

            var previousDirection = GetDirectionForDelta(previousDelta);
            var nextDirection = GetDirectionForDelta(nextDelta);
            return (previousDirection, nextDirection);
            

            Direction? GetDirectionForDelta(Vector3Int? delta)
            {
                if (delta == null)
                {
                    return null;
                }
                //else if (delta.Value.y != 0)
                //{
                //    // This means neighbour is above or below (not necessarily a staircase)
                //    // TODO: Work out this interaction
                //    // For now, use edge piece by treating this as a non neighbour
                //    return null; 
                //}
                else if (delta.Value.x < 0)
                {
                    //Debug.Log("West");
                    return Direction.West;
                }
                else if (delta.Value.x > 0)
                {
                    //Debug.Log("East");

                    return Direction.East;
                }
                else
                {
                    if (delta.Value.z < 0)
                    {
                        //Debug.Log("South");

                        return Direction.South;
                    }
                    else if (delta.Value.z > 0)
                    {
                        //Debug.Log("North");

                        return Direction.North;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private (GameObject, float) GetOrientedPrefabForDirections(Direction? previous, Direction? next)
        {
            if (previous == null && next == null)
            {
                return (null, 0);
                //throw new ArgumentException("Trying to place a Hallway piece with no neighbours");
            }

            if (previous == null || next == null)
            {
                var rotation = DirectionHelper.GetRotationAboutYAxis(previous ?? next.Value);
                return (_hallwayEnd, rotation);
            }

            if (previous.Value == next.Value)
            {
                // When the prev and next are the same, I believe we have a staircase
                // May need to deal with this interaction more carefully
                // For now, use same flow as single direction using hallway end 
                var rotation = DirectionHelper.GetRotationAboutYAxis(previous.Value);
                return (_hallwayEnd, rotation);
            }

            var directions = new List<Direction> { previous.Value, next.Value };

            if (directions.Contains(Direction.North) && directions.Contains(Direction.South))
            {
                //return (_hallway, Quaternion.AngleAxis(90, new Vector3(0, 1)));
                return (_hallway, 90);
            }
            else if (directions.Contains(Direction.East) && directions.Contains(Direction.West))
            {
                //return (_hallway, Quaternion.identity);
                return (_hallway, 0);
            }
            else if (directions.Contains(Direction.North) && directions.Contains(Direction.East))
            {
                //return (_hallwayL, Quaternion.AngleAxis(-90, new Vector3(0, 1)));
                return (_hallwayL, -90);
            }
            else if (directions.Contains(Direction.East) && directions.Contains(Direction.South))
            {
                //return (_hallwayL, Quaternion.identity);
                //return (_hallwayL, Quaternion.AngleAxis(180, new Vector3(0, 1)));
                return (_hallwayL, 180);
            }
            else if (directions.Contains(Direction.South) && directions.Contains(Direction.West))
            {
                //return (_hallwayL, Quaternion.AngleAxis(90, new Vector3(0, 1)));
                return (_hallwayL, 90);
            }
            else if (directions.Contains(Direction.West) && directions.Contains(Direction.North))
            {
                //return (_hallwayL, Quaternion.AngleAxis(180, new Vector3(0, 1)));
                return(_hallwayL, 180);

            }

            throw new ArgumentException("Hallway pieces in wrong configuration");
        }

        
    }
}