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

    public IEnumerable<TriangleCoord> TriangleCoords
    {
        get
        {
            // 0 to N inclusive because studs are at the corners of triangles
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col <= row; col++)
                {
                    yield return new TriangleCoord(row, col, true);
                    if (row != Size - 1)
                    {
                        yield return new TriangleCoord(row, col, false);
                    }
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

    public Vector2[] GetTriangleCorners(TriangleCoord triangle)
    {
        var result = new Vector2[3];

        if (triangle.PointUp)
        {
            result[0] = StudToPixel(new Stud(triangle.Row, triangle.Col));
            result[1] = StudToPixel(new Stud(triangle.Row + 1, triangle.Col));
            result[2] = StudToPixel(new Stud(triangle.Row + 1, triangle.Col + 1));
        }
        else
        {
            result[0] = StudToPixel(new Stud(triangle.Row + 1, triangle.Col + 1));
            result[1] = StudToPixel(new Stud(triangle.Row + 1, triangle.Col));
            result[2] = StudToPixel(new Stud(triangle.Row + 2, triangle.Col + 1));
        }

        return result;
    }

    public Vector2 GetTriangleCenter(TriangleCoord triangle)
    {
        var corners = GetTriangleCorners(triangle);
        return (corners[0] + corners[1] + corners[2]) / 3;
    }

    // Peg/triangle mapping moved to TriangleBoard

    // PegToPixel moved to TriangleBoard (uses TriangleBoard.PegToTriangleCoord and Geometry.GetTriangleCenter)
}