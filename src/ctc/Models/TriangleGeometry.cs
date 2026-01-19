namespace Models;

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

public class TriangleGeometry(int size, float spacing, float offsetX, float offsetY)
{
    public int Size { get; } = size;
    public float Spacing { get; } = spacing;
    public float OffsetX { get; } = offsetX;
    public float OffsetY { get; } = offsetY;

    private readonly float triangleHeight = spacing * MathF.Sqrt(3) / 2f;

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

        var row = triangle.Row;
        var index = triangle.Index;
        var col = index / 2;

        if (triangle.PointUp)
        {
            result[0] = VertexToPixel(new Vertex(row, col));
            result[1] = VertexToPixel(new Vertex(row + 1, col));
            result[2] = VertexToPixel(new Vertex(row + 1, col + 1));
        }
        else
        {
            result[0] = VertexToPixel(new Vertex(row, col + 1));
            result[1] = VertexToPixel(new Vertex(row, col));
            result[2] = VertexToPixel(new Vertex(row + 1, col + 1));
        }

        return result;
    }

    public Vector2 GetTriangleCenter(Position triangle)
    {
        var corners = GetTriangleCorners(triangle);
        return (corners[0] + corners[1] + corners[2]) / 3;
    }
}