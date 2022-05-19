using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System.Linq;
using Assets.Level.Models;
using Assets.Level.Builders;
using System;

public class Generator3D : MonoBehaviour {

    public enum CellType {
        None,
        Room,
        Hallway,
        Stairs
    }

    private LevelBuilder _levelBuilder;

    [SerializeField]
    Vector3Int size;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMaxSize;

    Random random;
    Grid3D<CellType> grid;
    List<Room> rooms;
    HashSet<Prim.Edge> selectedEdges;

    private int numNotAdded = 0;

    void Start() {
        random = new Random(0);
        grid = new Grid3D<CellType>(size, Vector3Int.zero);
        rooms = new List<Room>();

        _levelBuilder = FindObjectOfType<LevelBuilder>();

        PlaceRooms();
        var delaunay = Triangulate();

        if (!delaunay.Edges.Any())
        {
            throw new Exception("Level generation parameters resulted in delaunay with 0 edges. Try increasing room count or decreasing room max size");
        }

        CreateHallways(delaunay);
        var (hallways, staircases) = PathfindHallways();

        var level = new Level(rooms, hallways, staircases);
        _levelBuilder.Build(level);

        Debug.Log($"Generated level with {level.Rooms.Count} Rooms, {level.Hallways.Count} Hallways, {level.Staircases.Count} Staircases");        
    }

    void PlaceRooms() {
        for (int i = 0; i < roomCount; i++) {
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(1, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.Bounds.xMin < 0 || newRoom.Bounds.xMax >= size.x
                || newRoom.Bounds.yMin < 0 || newRoom.Bounds.yMax >= size.y
                || newRoom.Bounds.zMin < 0 || newRoom.Bounds.zMax >= size.z) {
                add = false;
            }

            if (add) {
                rooms.Add(newRoom);

                foreach (var pos in newRoom.Bounds.allPositionsWithin) {
                    grid[pos] = CellType.Room;
                }
            }
            else
            {
                numNotAdded++;
            }
        }
    }

    private Delaunay3D Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector3)room.Bounds.position + ((Vector3)room.Bounds.size) / 2, room));
        }

        return Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways(Delaunay3D delaunay) {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.125) {
                selectedEdges.Add(edge);
            }
        }
    }

    (ICollection<Hallway>, ICollection<Staircase>) PathfindHallways() 
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);
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

            var (pathHallways, pathStaircases) = BuildPath(path);
            hallways.AddRange(pathHallways);
            staircases.AddRange(pathStaircases);
        }

        return (hallways, staircases);
    }

    (ICollection<Hallway>, ICollection<Staircase>) BuildPath(List<Vector3Int> path)
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

