using UnityEngine;

public class Staircase : LevelComponent
{
    public Staircase(Vector3Int location, Vector3Int size) : base(location, size)
    {
    }

    public Staircase(Vector3Int previous, Vector3Int verticalOffset, Vector3Int horizontalOffset) : base()
    {
        AddPiece(new LevelComponentPiece(previous + horizontalOffset, null, previous + horizontalOffset * 2));
        AddPiece(new LevelComponentPiece(previous + horizontalOffset * 2, previous + horizontalOffset, previous + verticalOffset + horizontalOffset));
        AddPiece(new LevelComponentPiece(previous + verticalOffset + horizontalOffset, previous + horizontalOffset * 2, previous + verticalOffset + horizontalOffset * 2));
        AddPiece(new LevelComponentPiece(previous + verticalOffset + horizontalOffset * 2, previous + verticalOffset + horizontalOffset, null));
    }
}
