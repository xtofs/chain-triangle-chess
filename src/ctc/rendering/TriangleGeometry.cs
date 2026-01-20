namespace ctc.Rendering;

using System.Numerics;
using System.Runtime.CompilerServices;
using Models;

/// <summary>
/// Handles rendering of a triangular grid to pixel coordinates.
/// Inherits the logical grid operations from TriangleGrid.
/// </summary>
public class TriangleGeometry : TriangleGrid
{
    public float Spacing { get; }
    public float OffsetX { get; }
    public float OffsetY { get; }

    private readonly float triangleHeight;

    public TriangleGeometry(int size, float spacing, float offsetX, float offsetY) : base(size)
    {
        Spacing = spacing;
        OffsetX = offsetX;
        OffsetY = offsetY;
        triangleHeight = spacing * MathF.Sqrt(3) / 2f;
    }

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