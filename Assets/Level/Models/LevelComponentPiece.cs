using System;
using UnityEngine;

namespace Assets.Level.Models
{
    public class LevelComponentPiece : MonoBehaviour
    {
        public Guid ParentId { get; }
        public Vector3Int Location { get; }
        public LevelComponentPiece Previous { get; }
        public Vector3Int? Next { get; }

        public LevelComponentPiece(Guid parentId, Vector3Int location, LevelComponentPiece previous, Vector3Int? next)
        {
            ParentId = parentId;
            Location = location;
            Previous = previous;
            Next = next;
        }
    }
}