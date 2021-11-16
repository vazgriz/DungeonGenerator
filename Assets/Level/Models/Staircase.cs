using UnityEngine;

namespace Assets.Level.Models
{
    public class Staircase : LevelComponent
    {
        public Staircase(Vector3Int location, Vector3Int size) : base(location, size)
        {
        }

        public Staircase(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset) : base()
        {
            var first = new LevelComponentPiece(Id, previous + horizontalOffset, null, previous + horizontalOffset * 2);
            var second = new LevelComponentPiece(Id, previous + horizontalOffset * 2, first, previous + verticalOffset + horizontalOffset);
            var third = new LevelComponentPiece(Id, previous + verticalOffset + horizontalOffset, second, previous + verticalOffset + horizontalOffset * 2);
            var fourth = new LevelComponentPiece(Id, previous + verticalOffset + horizontalOffset * 2, third, null);

            AddPiece(first);
            AddPiece(second);
            AddPiece(third);
            AddPiece(fourth);
        }
    }
}