using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Models
{
    public class LevelComponent : MonoBehaviour
    {
        public Guid Id { get; } = Guid.NewGuid();
        public ICollection<LevelComponentPiece> Pieces { get; } = new List<LevelComponentPiece>();

        public BoundsInt Bounds { get; protected set; }

        public LevelComponent(Vector3Int location, Vector3Int size)
        {
            Bounds = new BoundsInt(location, size);
        }

        protected LevelComponent() { }

        public void AddPiece(LevelComponentPiece piece)
        {
            Pieces.Add(piece);
        }

        public static bool Intersect(LevelComponent a, LevelComponent b)
        {
            return !((a.Bounds.position.x >= (b.Bounds.position.x + b.Bounds.size.x)) || ((a.Bounds.position.x + a.Bounds.size.x) <= b.Bounds.position.x)
                || (a.Bounds.position.y >= (b.Bounds.position.y + b.Bounds.size.y)) || ((a.Bounds.position.y + a.Bounds.size.y) <= b.Bounds.position.y)
                || (a.Bounds.position.z >= (b.Bounds.position.z + b.Bounds.size.z)) || ((a.Bounds.position.z + a.Bounds.size.z) <= b.Bounds.position.z));
        }
    }
}