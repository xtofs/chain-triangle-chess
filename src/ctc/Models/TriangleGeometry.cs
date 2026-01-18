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

    public IEnumerable<Stud> Studs
    {
        get
        {
            // 0 to N inclusive because studs are at the corners of triangles
            for (int row = 0; row < Size + 1; row++)
            {
                for (int col = 0; col <= row; col++)
                {
                    yield return new Stud(row, col);
                }
            }
        }
    }

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

    public IEnumerable<Peg> GetPegs(int maxRow = 0)
    {
        if (maxRow == 0) { maxRow = Size; }
        // Pegs are numbered sequentially by row, with each row having 2*row + 1 pegs
        // Row 0: (0,0) - 1 peg
        // Row 1: (1,0), (1,1), (1,2) - 3 pegs
        // Row 2: (2,0), (2,1), (2,2), (2,3), (2,4) - 5 pegs
        // etc.
        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col <= 2 * row; col++)
            {
                yield return new Peg(row, col);
            }
        }
    }

    private readonly float triangleHeight = spacing * MathF.Sqrt(3) / 2f;


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

    public TriangleCoord PegToTriangleCoord(Peg peg)
    {
        var triangleCol = peg.Col / 2;

        if (peg.Col % 2 == 0)
        {
            return new TriangleCoord(peg.Row, triangleCol, true);
        }
        else
        {
            return new TriangleCoord(peg.Row - 1, triangleCol, false);
        }
    }

    public Peg TriangleCoordToPeg(TriangleCoord triangle)
    {
        if (triangle.PointUp)
        {
            return new Peg(triangle.Row, triangle.Col * 2);
        }
        else
        {
            return new Peg(triangle.Row + 1, triangle.Col * 2 + 1);
        }
    }

    public Vector2 PegToPixel(Peg peg)
    {
        // Direct calculation: center of the three studs that form the peg's triangle
        var triangleCol = peg.Col / 2;
        Vector2 stud1, stud2, stud3;

        if (peg.Col % 2 == 0)
        {
            // Point-up triangle
            stud1 = StudToPixel(new Stud(peg.Row, triangleCol));
            stud2 = StudToPixel(new Stud(peg.Row + 1, triangleCol));
            stud3 = StudToPixel(new Stud(peg.Row + 1, triangleCol + 1));
        }
        else
        {
            // Point-down triangle
            stud1 = StudToPixel(new Stud(peg.Row, triangleCol + 1));
            stud2 = StudToPixel(new Stud(peg.Row, triangleCol));
            stud3 = StudToPixel(new Stud(peg.Row + 1, triangleCol + 1));
        }

        return (stud1 + stud2 + stud3) / 3;
    }
}