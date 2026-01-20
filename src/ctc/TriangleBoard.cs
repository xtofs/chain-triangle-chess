namespace Models;

public class TriangleBoard(TriangleGrid grid)
{
    private readonly TriangleGrid _grid = grid;

    private readonly List<(Vertex, Vertex)> _bands = [];

    private readonly List<Piece> _pegs = [];

    static readonly string[] _colorRotation = ["red", "blue", "green"];

    private int _currentColorIndex = 0;

    public string CurrentColor => _colorRotation[_currentColorIndex];

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
        var trianglesToCheck = new HashSet<Position>(_grid.TrianglePositions);

        foreach (var triangle in trianglesToCheck)
        {
            // Only add a peg if it's surrounded AND we haven't already created one there
            if (IsTriangleSurrounded(triangle) && !_pegs.Any(p => p.Position == triangle))
            {
                var piece = new Piece(triangle, _colorRotation[_currentColorIndex]);
                _pegs.Add(piece);
            }
        }

        // Rotate to the next player's color
        _currentColorIndex = (_currentColorIndex + 1) % _colorRotation.Length;
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
        // Check all possible triangles in the grid
        foreach (var triangle in _grid.TrianglePositions)
        {
            var vertices = _grid.GetTriangleVertices(triangle);
            // Check if this triangle has v1 and v2 as an edge
            if (_grid.HasEdge(vertices, v1, v2))
            {
                yield return triangle;
            }
        }
    }

    /// <summary>
    /// Check if a triangle is surrounded by bands on all three sides.
    /// </summary>
    public bool IsTriangleSurrounded(Position triangle)
    {
        var vertices = _grid.GetTriangleVertices(triangle);

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
                var vertex1 = path[i];
                var vertex2 = path[i + 1];
                if ((vertex1 == v1 && vertex2 == v2) || (vertex1 == v2 && vertex2 == v1))
                {
                    return true;
                }
            }
        }
        return false;
    }
}