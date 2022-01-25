using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Models
{
    public class Level : MonoBehaviour
    {
        public Level(ICollection<Room> rooms, ICollection<Hallway> hallways, ICollection<Staircase> staircases)
        {
            Rooms = rooms;
            Hallways = hallways;
            Staircases = staircases;
        }

        public ICollection<Room> Rooms { get; }
        public ICollection<Hallway> Hallways { get; }
        public ICollection<Staircase> Staircases { get; }
    
    }
}
