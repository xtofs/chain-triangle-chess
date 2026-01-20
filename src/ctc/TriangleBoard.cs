namespace Models;

public class TriangleBoard(TriangleGeometry geometry)
{
    private List<(Vertex, Vertex)> _bands = [];
    private List<Piece> _pegs = [];

    public IReadOnlyList<(Vertex, Vertex)> Bands => _bands;
    public IReadOnlyList<Piece> Pegs => _pegs;

    public void AddBand(Vertex from, Vertex to)
    {
        // Generate the band path (4 vertices creating 3 edges)
        var path = GetBandPath(from, to);

        // Store the band
        _bands.Add((from, to));

        // After adding each band, check ALL triangles to see which are now surrounded
        // (A triangle might have had 2 edges covered by previous bands, and the new band
        // completes it, but the new band's edges might not directly touch it)
        var trianglesToCheck = new HashSet<Position>(geometry.TrianglePositions);

        foreach (var triangle in trianglesToCheck)
        {
            // Only add a peg if it's surrounded AND we haven't already created one there
            if (IsTriangleSurrounded(triangle) && !_pegs.Any(p => p.Position == triangle))
            {
                var piece = new Piece(triangle);
                _pegs.Add(piece);
            }
        }
    }

    /// <summary>
    /// Generate the band path from start to end vertex.
    /// Bands span 4 vertices, creating 3 edges.
    /// Orientation is determined by which coordinates change.
    /// </summary>
    public List<Vertex> GetBandPath(Vertex from, Vertex to)
    {
        var path = new List<Vertex> { from };

        // Determine orientation and delta
        int deltaRow, deltaCol;

        if (from.Row == to.Row)
        {
            // Horizontal: same row, different col
            deltaRow = 0;
            deltaCol = from.Col < to.Col ? 1 : -1;
        }
        else if (from.Col == to.Col)
        {
            // Anti-diagonal: same col, different row
            deltaRow = from.Row < to.Row ? 1 : -1;
            deltaCol = 0;
        }
        else if ((from.Row - from.Col) == (to.Row - to.Col))
        {
            // Diagonal: row - col is constant
            deltaRow = from.Row < to.Row ? 1 : -1;
            deltaCol = from.Col < to.Col ? 1 : -1;
        }
        else
        {
            throw new ArgumentException($"Invalid band path from {from} to {to}");
        }

        // Generate intermediate vertices
        var current = from;
        while (current != to)
        {
            current = new Vertex(current.Row + deltaRow, current.Col + deltaCol);
            path.Add(current);
        }

        return path;
    }

    /// <summary>
    /// Get all triangles that have the given two vertices as an edge.
    /// </summary>
    public IEnumerable<Position> GetTrianglesTouching(Vertex v1, Vertex v2)
    {
        // Check all possible triangles in the geometry
        foreach (var triangle in geometry.TrianglePositions)
        {
            var vertices = GetTriangleVertices(triangle);
            // Check if this triangle has v1 and v2 as an edge
            if (HasEdge(vertices, v1, v2))
            {
                yield return triangle;
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
    /// Check if a triangle is surrounded by bands on all three sides.
    /// </summary>
    public bool IsTriangleSurrounded(Position triangle)
    {
        var vertices = GetTriangleVertices(triangle);

        // Check all three edges
        for (int i = 0; i < 3; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % 3];

            if (!HasBandBetween(v1, v2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Check if there's a band edge between two vertices (in either direction).
    /// A band consists of 4 vertices creating 3 edges, so we need to check if v1 and v2
    /// are consecutive vertices in any band's path.
    /// </summary>
    public bool HasBandBetween(Vertex v1, Vertex v2)
    {
        // Check each band to see if v1 and v2 are consecutive in its path
        foreach (var (from, to) in _bands)
        {
            var path = GetBandPath(from, to);
            for (int i = 0; i < path.Count - 1; i++)
            {
                var pathV1 = path[i];
                var pathV2 = path[i + 1];
                if ((pathV1 == v1 && pathV2 == v2) || (pathV1 == v2 && pathV2 == v1))
                {
                    return true;
                }
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
        foreach (var triangle in geometry.TrianglePositions)
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
    private bool IsValidVertex(Vertex v)
    {
        // lower triangular condition: 0 <= col <= row <= size
        return v.Row >= 0 && v.Row <= geometry.Size && v.Col >= 0 && v.Col <= v.Row;
    }
}