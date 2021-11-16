using Assets.Level.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Level.Builders
{
    public class RoomBuilder : MonoBehaviour
    {
        [SerializeField]
        GameObject centre;
        [SerializeField]
        GameObject southWestCorner;
        [SerializeField]
        GameObject southEastCorner;
        [SerializeField]
        GameObject northWestCorner;
        [SerializeField]
        GameObject northEastCorner;
        [SerializeField]
        GameObject southEdge;
        [SerializeField]
        GameObject northEdge;
        [SerializeField]
        GameObject westEdge;
        [SerializeField]
        GameObject eastEdge;

        public void BuildMany(ICollection<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Build(room);
            }
        }

        public void Build(Room room)
        {
            Build(room.Bounds.position, room.Bounds.size);
        }

        private void Build(Vector3Int location, Vector3Int size)
        {
            var width = location.x + size.x;
            var depth = location.z + size.z;
            var xOrigin = location.x;
            var zOrigin = location.z;

            for (var x = xOrigin; x <= width; x++)
            {
                for (var z = zOrigin; z <= depth; z++)
                {
                    var modularPrefab = GetModularPrefab(xOrigin, zOrigin, x, z, width, depth);
                    Instantiate(modularPrefab, new Vector3Int(x, location.y, z), Quaternion.identity);
                }
            }
        }

        private GameObject GetModularPrefab(int minX, int minZ, int x, int z, int maxX, int maxZ)
        {
            if (z == minZ)
            {
                if (x == minX)
                {
                    return southWestCorner;
                }
                else if (x == maxX)
                {
                    return southEastCorner;
                }
                else
                {
                    return southEdge;
                }
            }
            else if (z == maxZ)
            {
                if (x == minX)
                {
                    return northWestCorner;
                }
                else if (x == maxX)
                {
                    return northEastCorner;
                }
                else
                {
                    return northEdge;
                }
            }
            else if (x == minX)
            {
                return westEdge;
            }
            else if (x == maxX)
            {
                return eastEdge;
            }

            return centre;
        }
    }
}