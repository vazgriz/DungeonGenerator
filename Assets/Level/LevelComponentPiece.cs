using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComponentPiece : MonoBehaviour
{
    public Vector3Int Location { get; }
    public Vector3Int? Previous { get; }
    public Vector3Int? Next { get; }

    public LevelComponentPiece(Vector3Int location, Vector3Int? previous, Vector3Int? next)
    {
        Location = location;
        Previous = previous;
        Next = next;
    }
}
