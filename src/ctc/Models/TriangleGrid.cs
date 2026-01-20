namespace Models;

/// <summary>
/// Represents the logical structure of a triangular grid.
/// Pure mathematical/logical grid operations with no rendering concerns.
/// </summary>
public class TriangleGrid(int size)
{
    public int Size { get; } = size;

    public IEnumerable<Position> TrianglePositions
    {
        get
        {
            // enumerate compact positions: for each row, indices 0..2*row
            for (int row = 0; row < Size; row++)
            {
                for (int index = 0; index <= 2 * row; index++)
                {
                    yield return new Position(row, index);
                }
            }
        }
    }

    /// <summary>
    /// Get the three vertices of a triangle.
    /// </summary>
    public Vertex[] GetTriangleVertices(Position triangle)
    {
        var row = triangle.Row;
        var index = triangle.Index;
        var col = index / 2;

        if (triangle.PointUp)
        {
            return new[]
            {
                new Vertex(row, col),
                new Vertex(row + 1, col),
                new Vertex(row + 1, col + 1)
            };
        }
        else
        {
            return new[]
            {
                new Vertex(row, col + 1),
                new Vertex(row, col),
                new Vertex(row + 1, col + 1)
            };
        }
    }

    /// <summary>
    /// Check if a triangle has an edge between two vertices.
    /// </summary>
    public bool HasEdge(Vertex[] vertices, Vertex v1, Vertex v2)
    {
        for (int i = 0; i < 3; i++)
        {
            var a = vertices[i];
            var b = vertices[(i + 1) % 3];
            if ((a == v1 && b == v2) || (a == v2 && b == v1))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get all vertices that are adjacent (form an edge) with the given vertex.
    /// </summary>
    public IEnumerable<Vertex> GetAdjacentVertices(Vertex v)
    {
        var adjacent = new HashSet<Vertex>();

        // Check all triangles to find which ones contain this vertex
        foreach (var triangle in TrianglePositions)
        {
            var vertices = GetTriangleVertices(triangle);
            if (vertices.Contains(v))
            {
                // Add all other vertices of this triangle as adjacent
                foreach (var vertex in vertices)
                {
                    if (vertex != v)
                    {
                        adjacent.Add(vertex);
                    }
                }
            }
        }

        return adjacent;
    }

    /// <summary>
    /// Get all vertices reachable in exactly 3 hops from the given vertex,
    /// following only valid triangle edges (adjacent vertices).
    /// </summary>
    public IEnumerable<Vertex> GetReachableVertices(Vertex from)
    {
        var reachable = new HashSet<Vertex>();
        var visited = new HashSet<Vertex>();
        var toVisit = new Queue<(Vertex, int)>();
        toVisit.Enqueue((from, 0));
        visited.Add(from);

        while (toVisit.Count > 0)
        {
            var (current, distance) = toVisit.Dequeue();

            if (distance == 3)
            {
                reachable.Add(current);
                continue;
            }

            if (distance < 3)
            {
                // Get all adjacent vertices (those connected by triangle edges)
                foreach (var adjacent in GetAdjacentVertices(current))
                {
                    if (!visited.Contains(adjacent))
                    {
                        visited.Add(adjacent);
                        toVisit.Enqueue((adjacent, distance + 1));
                    }
                }
            }
        }

        return reachable.OrderBy(v => v.Row).ThenBy(v => v.Col);
    }

    /// <summary>
    /// Check if a vertex is valid (within the bounds of the triangular grid).
    /// </summary>
    public bool IsValidVertex(Vertex v)
    {
        // lower triangular condition: 0 <= col <= row <= size
        return v.Row >= 0 && v.Row <= Size && v.Col >= 0 && v.Col <= v.Row;
    }
}
