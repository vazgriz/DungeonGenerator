using System;
using UnityEngine;

namespace Assets.Level.Models
{
    public class LevelComponentPiece : MonoBehaviour
    {
        public Guid ParentId { get; }
        public Vector3Int Location { get; }
        public LevelComponentPiece Previous { get; }
        public Vector3Int? Next { get; set; }

        public LevelComponentPiece(Guid parentId, Vector3Int location, Vector3Int? next)
        {
            ParentId = parentId;
            Location = location;
            Next = next;
        }

        public LevelComponentPiece(Guid parentId, Vector3Int location, Vector3Int? next, ref LevelComponentPiece previous) : this(parentId, location, next)
        {            
            Previous = previous;
        }
    }
}