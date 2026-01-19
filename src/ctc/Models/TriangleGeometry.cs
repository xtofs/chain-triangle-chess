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

    // Stud enumeration moved to TriangleBoard

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
    /// Convert a stud coordinate to pixel coordinates.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Vector2 StudToPixel(Stud stud)
    {
        // in a triangular grid:
        //   each col (x) shifts right by spacing
        //   each row (y) shifts down by spacing * sqrt(3)/2, the triangle height
        //   each row also shifts left by (row * spacing/2) 
        //       for the all up triangular layout around the vertical center line with the pointy end up
        var x = OffsetX + stud.Col * Spacing + (Size - stud.Row) * Spacing / 2.0f;
        var y = OffsetY + stud.Row * triangleHeight;

        return new Vector2(x, y);
    }

    public Stud PixelToStud(Vector2 point)
    {
        var x = point.X - OffsetX;
        var y = point.Y - OffsetY;
        int row = (int)MathF.Round(y / triangleHeight);
        double rowOffset = (row * Spacing) / 2;
        int col = (int)Math.Round((x + rowOffset) / Spacing);

        return new Stud(row, col);
    }

    public Vector2[] GetTriangleCorners(Position triangle)
    {
        var result = new Vector2[3];

        // derive the equivalent TriangleCoord (row,col,pointUp) from compact Position
        var row = triangle.Row;
        var index = triangle.Index;
        var col = index / 2;

        if (triangle.PointUp)
        {
            // corresponds to TriangleCoord(row, col, true)
            result[0] = StudToPixel(new Stud(row, col));
            result[1] = StudToPixel(new Stud(row + 1, col));
            result[2] = StudToPixel(new Stud(row + 1, col + 1));
        }
        else
        {
            // corresponds to TriangleCoord(row-1, col, false)
            result[0] = StudToPixel(new Stud(row, col + 1));
            result[1] = StudToPixel(new Stud(row, col));
            result[2] = StudToPixel(new Stud(row + 1, col + 1));
        }

        return result;
    }

    public Vector2 GetTriangleCenter(Position triangle)
    {
        var corners = GetTriangleCorners(triangle);
        return (corners[0] + corners[1] + corners[2]) / 3;
    }

    // Peg/triangle mapping moved to TriangleBoard

    // PegToPixel moved to TriangleBoard (uses TriangleBoard.PegToTriangleCoord and Geometry.GetTriangleCenter)
}