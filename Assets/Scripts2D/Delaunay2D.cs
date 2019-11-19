/* Adapted from https://github.com/Bl4ckb0ne/delaunay-triangulation

Copyright (c) 2015-2019 Simon Zeni (simonzeni@gmail.com)


Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:


The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.


THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class Delaunay2D {
    public class Triangle : IEquatable<Triangle> {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        public bool IsBad { get; set; }

        public Triangle() {

        }

        public Triangle(Vertex a, Vertex b, Vertex c) {
            A = a;
            B = b;
            C = c;
        }

        public bool ContainsVertex(Vector3 v) {
            return Vector3.Distance(v, A.Position) < 0.01f
                || Vector3.Distance(v, B.Position) < 0.01f
                || Vector3.Distance(v, C.Position) < 0.01f;
        }

        public bool CircumCircleContains(Vector3 v) {
            Vector3 a = A.Position;
            Vector3 b = B.Position;
            Vector3 c = C.Position;

            float ab = a.sqrMagnitude;
            float cd = b.sqrMagnitude;
            float ef = c.sqrMagnitude;

            float circumX = (ab * (c.y - b.y) + cd * (a.y - c.y) + ef * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
            float circumY = (ab * (c.x - b.x) + cd * (a.x - c.x) + ef * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));

            Vector3 circum = new Vector3(circumX / 2, circumY / 2);
            float circumRadius = Vector3.SqrMagnitude(a - circum);
            float dist = Vector3.SqrMagnitude(v - circum);
            return dist <= circumRadius;
        }

        public static bool operator ==(Triangle left, Triangle right) {
            return (left.A == right.A || left.A == right.B || left.A == right.C)
                && (left.B == right.A || left.B == right.B || left.B == right.C)
                && (left.C == right.A || left.C == right.B || left.C == right.C);
        }

        public static bool operator !=(Triangle left, Triangle right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Triangle t) {
                return this == t;
            }

            return false;
        }

        public bool Equals(Triangle t) {
            return this == t;
        }

        public override int GetHashCode() {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
    }

    public class Edge {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        public bool IsBad { get; set; }

        public Edge() {

        }

        public Edge(Vertex u, Vertex v) {
            U = u;
            V = v;
        }

        public static bool operator ==(Edge left, Edge right) {
            return (left.U == right.U || left.U == right.V)
                && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge left, Edge right) {
            return !(left == right);
        }

        public override bool Equals(object obj) {
            if (obj is Edge e) {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e) {
            return this == e;
        }

        public override int GetHashCode() {
            return U.GetHashCode() ^ V.GetHashCode();
        }

        public static bool AlmostEqual(Edge left, Edge right) {
            return Delaunay2D.AlmostEqual(left.U, right.U) && Delaunay2D.AlmostEqual(left.V, right.V)
                || Delaunay2D.AlmostEqual(left.U, right.V) && Delaunay2D.AlmostEqual(left.V, right.U);
        }
    }

    static bool AlmostEqual(float x, float y) {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
            || Mathf.Abs(x - y) < float.MinValue;
    }

    static bool AlmostEqual(Vertex left, Vertex right) {
        return AlmostEqual(left.Position.x, right.Position.x) && AlmostEqual(left.Position.y, right.Position.y);
    }

    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }

    Delaunay2D() {
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
    }

    public static Delaunay2D Triangulate(List<Vertex> vertices) {
        Delaunay2D delaunay = new Delaunay2D();
        delaunay.Vertices = new List<Vertex>(vertices);
        delaunay.Triangulate();

        return delaunay;
    }

    void Triangulate() {
        float minX = Vertices[0].Position.x;
        float minY = Vertices[0].Position.y;
        float maxX = minX;
        float maxY = minY;

        foreach (var vertex in Vertices) {
            if (vertex.Position.x < minX) minX = vertex.Position.x;
            if (vertex.Position.x > maxX) maxX = vertex.Position.x;
            if (vertex.Position.y < minY) minY = vertex.Position.y;
            if (vertex.Position.y > maxY) maxY = vertex.Position.y;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy) * 2;

        Vertex p1 = new Vertex(new Vector2(minX - 1         , minY - 1          ));
        Vertex p2 = new Vertex(new Vector2(minX - 1         , maxY + deltaMax   ));
        Vertex p3 = new Vertex(new Vector2(maxX + deltaMax  , minY - 1          ));

        Triangles.Add(new Triangle(p1, p2, p3));

        foreach (var vertex in Vertices) {
            List<Edge> polygon = new List<Edge>();

            foreach (var t in Triangles) {
                if (t.CircumCircleContains(vertex.Position)) {
                    t.IsBad = true;
                    polygon.Add(new Edge(t.A, t.B));
                    polygon.Add(new Edge(t.B, t.C));
                    polygon.Add(new Edge(t.C, t.A));
                }
            }

            Triangles.RemoveAll((Triangle t) => t.IsBad);

            for (int i = 0; i < polygon.Count; i++) {
                for (int j = i + 1; j < polygon.Count; j++) {
                    if (Edge.AlmostEqual(polygon[i], polygon[j])) {
                        polygon[i].IsBad = true;
                        polygon[j].IsBad = true;
                    }
                }
            }

            polygon.RemoveAll((Edge e) => e.IsBad);

            foreach (var edge in polygon) {
                Triangles.Add(new Triangle(edge.U, edge.V, vertex));
            }
        }

        Triangles.RemoveAll((Triangle t) => t.ContainsVertex(p1.Position) || t.ContainsVertex(p2.Position) || t.ContainsVertex(p3.Position));

        HashSet<Edge> edgeSet = new HashSet<Edge>();

        foreach (var t in Triangles) {
            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);

            if (edgeSet.Add(ab)) {
                Edges.Add(ab);
            }

            if (edgeSet.Add(bc)) {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca)) {
                Edges.Add(ca);
            }
        }
    }
}
