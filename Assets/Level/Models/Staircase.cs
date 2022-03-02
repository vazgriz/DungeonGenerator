using Assets.World.Models;
using System;
using UnityEngine;

namespace Assets.Level.Models
{
    public class Staircase : LevelComponent
    {

        public Vector3Int Previous { get; }
        public Vector3Int VerticalOffset { get; }
        public Vector3Int HorizontalOffset { get; }
        public bool IsUp { get; }
        public Direction Direction { get; }

        public Staircase(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset) : base()
        {
            Previous = previous;
            VerticalOffset = verticalOffset;
            HorizontalOffset = horizontalOffset;

            IsUp = VerticalOffset.y > 0;
            Direction = HorizontalOffset.x == 0 ? HorizontalOffset.z > 0 ? Direction.North : Direction.South
                                                    : HorizontalOffset.x > 0 ? Direction.East : Direction.West;

            if (verticalOffset.y == 0)
            {
                Debug.Log("Cannot create a staircase with no elevation change");
                throw new ArgumentException("Cannot create a staircase with no elevation change");
            }

            if (horizontalOffset.x != 0 && horizontalOffset.z != 0)
            {
                Debug.Log($"Staircase has nonzero x and z offsets: {horizontalOffset.x}, {horizontalOffset.z}");
                throw new ArgumentException("Can't create a staircase with ambiguous direction");
            }

            AddPieces(previous, verticalOffset, horizontalOffset);
        }

        private void AddPieces(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset)
        {
            var first = new LevelComponentPiece(Id, previous + horizontalOffset, previous + horizontalOffset * 2);
            var second = new LevelComponentPiece(Id, previous + horizontalOffset * 2, previous + verticalOffset + horizontalOffset, ref first);
            var third = new LevelComponentPiece(Id, previous + verticalOffset + horizontalOffset, previous + verticalOffset + horizontalOffset * 2, ref second);
            var fourth = new LevelComponentPiece(Id, previous + verticalOffset + horizontalOffset * 2, null, ref third);

            AddPiece(first);
            AddPiece(second);
            AddPiece(third);
            AddPiece(fourth);
        }
    }
}