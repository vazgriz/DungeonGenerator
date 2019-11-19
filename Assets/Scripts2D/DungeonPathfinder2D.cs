using System;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class DungeonPathfinder2D {
    public class Node {
        public Vector2Int Position { get; private set; }
        public Node Previous { get; set; }
        public float Cost { get; set; }

        public Node(Vector2Int position) {
            Position = position;
        }
    }

    public struct PathCost {
        public bool traversable;
        public float cost;
    }

    static readonly Vector2Int[] neighbors = {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
    };

    Grid2D<Node> grid;
    SimplePriorityQueue<Node, float> queue;
    HashSet<Node> closed;
    Stack<Vector2Int> stack;

    public DungeonPathfinder2D(Vector2Int size) {
        grid = new Grid2D<Node>(size, Vector2Int.zero);

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector2Int>();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                    grid[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    void ResetNodes() {
        var size = grid.Size;

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                var node = grid[x, y];
                node.Previous = null;
                node.Cost = float.PositiveInfinity;
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Func<Node, Node, PathCost> costFunction) {
        ResetNodes();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        grid[start].Cost = 0;
        queue.Enqueue(grid[start], 0);

        while (queue.Count > 0) {
            Node node = queue.Dequeue();
            closed.Add(node);

            if (node.Position == end) {
                return ReconstructPath(node);
            }

            foreach (var offset in neighbors) {
                if (!grid.InBounds(node.Position + offset)) continue;
                var neighbor = grid[node.Position + offset];
                if (closed.Contains(neighbor)) continue;

                var pathCost = costFunction(node, neighbor);
                if (!pathCost.traversable) continue;

                float newCost = node.Cost + pathCost.cost;

                if (newCost < neighbor.Cost) {
                    neighbor.Previous = node;
                    neighbor.Cost = newCost;

                    if (queue.TryGetPriority(node, out float existingPriority)) {
                        queue.UpdatePriority(node, newCost);
                    } else {
                        queue.Enqueue(neighbor, neighbor.Cost);
                    }
                }
            }
        }

        return null;
    }

    List<Vector2Int> ReconstructPath(Node node) {
        List<Vector2Int> result = new List<Vector2Int>();

        while (node != null) {
            stack.Push(node.Position);
            node = node.Previous;
        }

        while (stack.Count > 0) {
            result.Add(stack.Pop());
        }

        return result;
    }
}
