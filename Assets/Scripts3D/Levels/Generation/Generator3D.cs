using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System.Linq;
using Assets.Level.Models;
using System;

public class Generator3D : MonoBehaviour {

    public enum CellType 
    {
        None,
        Room,
        Hallway,
        Stairs
    }

    private Random _random;

    void Start() 
    {
        _random = new Random(0);
    }

    public Level Generate(Vector3Int size, int roomCount, Vector3Int roomMaxSize)
    {
        var grid = new Grid3D<CellType>(size, Vector3Int.zero);

        var rooms = PlaceRooms(size, roomCount, roomMaxSize, ref grid);
        var delaunay = Triangulate(rooms);
        
        var selectedEdges = CreateHallways(delaunay);
        var (hallways, staircases) = PathfindHallways(size, selectedEdges, grid);

        var level = new Level(rooms, hallways, staircases);
        Debug.Log($"Generated level with {level.Rooms.Count} Rooms, {level.Hallways.Count} Hallways, {level.Staircases.Count} Staircases");

        return level;
    }

    private ICollection<Room> PlaceRooms(Vector3Int size, int roomCount, Vector3Int roomMaxSize, ref Grid3D<CellType> grid)
    {
        var rooms = new List<Room>();

        for (int i = 0; i < roomCount; i++) 
        {
            Vector3Int location = new Vector3Int(
                _random.Next(0, size.x),
                _random.Next(0, size.y),
                _random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                _random.Next(1, roomMaxSize.x + 1),
                _random.Next(1, roomMaxSize.y + 1),
                _random.Next(1, roomMaxSize.z + 1)
            );

            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            if (RoomHasValidDimensions(newRoom) && !RoomIntersectsAnyExistingRoom(newRoom)) 
            {
                rooms.Add(newRoom);

                foreach (var pos in newRoom.Bounds.allPositionsWithin) 
                {
                    grid[pos] = CellType.Room;
                }
            }
        }

        return rooms;

        bool RoomHasValidDimensions(Room room)
        {
            return room.Bounds.xMin < 0 || room.Bounds.xMax >= size.x
                || room.Bounds.yMin < 0 || room.Bounds.yMax >= size.y
                || room.Bounds.zMin < 0 || room.Bounds.zMax >= size.z;
        }
        
        bool RoomIntersectsAnyExistingRoom(Room room)
        {
            return rooms.Any(r => LevelComponent.Intersect(r, room));
        }
    }

    private Delaunay3D Triangulate(ICollection<Room> rooms) 
    {
        var vertices = rooms.Select(room => new Vertex<Room>(room.Bounds.position + ((Vector3)room.Bounds.size) / 2, room));

        var delaunay = Delaunay3D.Triangulate(vertices);

        if (!delaunay.Edges.Any())
        {
            throw new Exception("Level generation parameters resulted in delaunay with 0 edges. Try increasing room count or decreasing room max size");
        }

        return delaunay;
    }

    private HashSet<Prim.Edge> CreateHallways(Delaunay3D delaunay)
    {
        var edges = delaunay.Edges.Select(e => new Prim.Edge(e.U, e.V)).ToList();

        var minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        var selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) 
        {
            if (_random.NextDouble() < 0.125) 
            {
                selectedEdges.Add(edge);
            }
        }

        return selectedEdges;
    }

    private (ICollection<Hallway>, ICollection<Staircase>) PathfindHallways(Vector3Int size, HashSet<Prim.Edge> selectedEdges, Grid3D<CellType> grid) 
    {
        var aStar = new DungeonPathfinder3D(size);
        var hallways = new List<Hallway>();
        var staircases = new List<Staircase>();

       
        foreach (var edge in selectedEdges) 
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.Bounds.center;
            var endPosf = endRoom.Bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => 
            {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0) {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs) {
                        return pathCost;
                    } else if (grid[b.Position] == CellType.Room) {
                        pathCost.cost += 5;
                    } else if (grid[b.Position] == CellType.None) {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                } 
                else 
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway))
                    {
                        return pathCost;
                    }

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    var verticalOffset = new Vector3Int(0, delta.y, 0);
                    var horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset)) 
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None) 
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            var (pathHallways, pathStaircases) = BuildPath(path, grid);
            hallways.AddRange(pathHallways);
            staircases.AddRange(pathStaircases);
        }

        return (hallways, staircases);
    }

    private (ICollection<Hallway>, ICollection<Staircase>) BuildPath(List<Vector3Int> path, Grid3D<CellType> grid)
    {
        var hallways = new List<Hallway>();
        var staircases = new List<Staircase>();

        if (path != null)
        {
            var hallway = new Hallway();
            LevelComponentPiece previousPiece = null;
            for (int i = 0; i < path.Count; i++)
            {
                var current = path[i];
                if (i > 0)
                {
                    var prev = path[i - 1];

                    var delta = current - prev;
                    if (delta.y != 0)
                    {
                        int xDir = Mathf.Clamp(delta.x, -1, 1);
                        int zDir = Mathf.Clamp(delta.z, -1, 1);
                        Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                        Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                        grid[prev + horizontalOffset] = CellType.Stairs;
                        grid[prev + horizontalOffset * 2] = CellType.Stairs;
                        grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                        grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;

                        var staircase = new Staircase(prev, verticalOffset, horizontalOffset);
                        staircases.Add(staircase);
                            
                        if (previousPiece != null)
                        {
                            previousPiece.Next = staircase.Pieces.Single(x => x.Previous == null).Location;
                        }
                        previousPiece = staircase.Pieces.Single(x => x.Next == null);

                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }

                if (grid[current] == CellType.Hallway || grid[current] == CellType.None)
                {
                    LevelComponentPiece piece;
                    if (i == path.Count - 1)
                    {
                        // Last
                        piece = new LevelComponentPiece(hallway.Id, path[i], null, ref previousPiece);
                    }
                    else
                    {
                        piece = new LevelComponentPiece(hallway.Id, path[i], path[i + 1], ref previousPiece);
                    }
                    hallway.AddPiece(piece);

                    if (previousPiece != null)
                    {
                        previousPiece.Next = piece.Location;
                    }
                    previousPiece = piece;
                }
            }

            hallways.Add(hallway);
        }

        return (hallways, staircases);
    }
}

