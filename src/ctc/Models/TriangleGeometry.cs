namespace Models;

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

/// <summary>
/// Handles rendering of a triangular grid to pixel coordinates.
/// Composes a TriangleGrid for logical operations.
/// </summary>
public class TriangleGeometry : ITriangleGrid
{
    private readonly TriangleGrid _grid;

    public int Size => _grid.Size;
    public IEnumerable<Position> TrianglePositions => _grid.TrianglePositions;

    public float Spacing { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    private readonly float triangleHeight;

    public TriangleGeometry(int size, float spacing, float offsetX, float offsetY)
    {
        _grid = new TriangleGrid(size);
        Spacing = spacing;
        OffsetX = offsetX;
        OffsetY = offsetY;
        triangleHeight = spacing * MathF.Sqrt(3) / 2f;
    }

    // Delegate logical grid operations to the grid
    public Vertex[] GetTriangleVertices(Position triangle) => _grid.GetTriangleVertices(triangle);
    public bool HasEdge(Vertex[] vertices, Vertex v1, Vertex v2) => _grid.HasEdge(vertices, v1, v2);
    public IEnumerable<Vertex> GetAdjacentVertices(Vertex v) => _grid.GetAdjacentVertices(v);
    public IEnumerable<Vertex> GetReachableVertices(Vertex from) => _grid.GetReachableVertices(from);
    public bool IsValidVertex(Vertex v) => _grid.IsValidVertex(v);

    /// <summary>
    /// Convert a vertex coordinate to pixel coordinates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Vector2 VertexToPixel(Vertex vertex)
    {
        // in a triangular grid:
        //   each col (x) shifts right by spacing
        //   each row (y) shifts down by spacing * sqrt(3)/2, the triangle height
        //   each row also shifts left by (row * spacing/2) 
        //       for the all up triangular layout around the vertical center line with the pointy end up
        var x = OffsetX + vertex.Col * Spacing + (Size - vertex.Row) * Spacing / 2.0f;
        var y = OffsetY + vertex.Row * triangleHeight;

        return new Vector2(x, y);
    }

    public Vertex PixelToStud(Vector2 point)
    {
        var x = point.X - OffsetX;
        var y = point.Y - OffsetY;
        int row = (int)MathF.Round(y / triangleHeight);
        double rowOffset = (row * Spacing) / 2;
        int col = (int)Math.Round((x + rowOffset) / Spacing);

        return new Vertex(row, col);
    }

    public Vector2[] GetTriangleCorners(Position triangle)
    {
        var result = new Vector2[3];

        var vertices = GetTriangleVertices(triangle);
        result[0] = VertexToPixel(vertices[0]);
        result[1] = VertexToPixel(vertices[1]);
        result[2] = VertexToPixel(vertices[2]);

        return result;
    }

    public Vector2 GetTriangleCenter(Position triangle)
    {
        var corners = GetTriangleCorners(triangle);
        return (corners[0] + corners[1] + corners[2]) / 3;
    }
}