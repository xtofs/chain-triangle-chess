namespace Models;

using System.Collections.Generic;
using System.Numerics;

public class TriangleBoard(TriangleGeometry geom)
{
    private readonly int _size = geom.Size;

    public TriangleGeometry Geometry { get; } = geom;

    public IEnumerable<Stud> Studs
    {
        get
        {
            for (int row = 0; row < _size + 1; row++)
            {
                for (int col = 0; col <= row; col++)
                {
                    yield return new Stud(row, col);
                }
            }
        }
    }

    public IEnumerable<Peg> GetPegs(int maxRow = 0)
    {
        if (maxRow == 0) { maxRow = _size; }
        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col <= 2 * row; col++)
            {
                yield return new Peg(row, col);
            }
        }
    }

    public Position PegToPosition(Peg peg)
    {
        var triangleCol = peg.Col / 2;

        if (peg.Col % 2 == 0)
        {
            return new Position(peg.Row, triangleCol * 2);
        }
        else
        {
            return new Position(peg.Row, triangleCol * 2 + 1);
        }
    }

    public Peg PositionToPeg(Position p)
    {
        return new Peg(p.Row, p.Index);
    }

    public Vector2 PegToPixel(Peg peg)
    {
        var position = PegToPosition(peg);
        return Geometry!.GetTriangleCenter(position);
    }

    // public IEnumerable<TriangleCoord> EnumerateTriangles(int gridSize)
    // {
    //     for (int row = 0; row < gridSize; row++)
    //     {
    //         for (int col = 0; col <= row; col++)
    //         {
    //             // Point-up triangle exists if there's a row below
    //             if (row < gridSize - 1)
    //             {
    //                 yield return new TriangleCoord(row, col, pointsUp: true);
    //             }

    //             // Point-down triangle exists if not at the right edge
    //             if (col < row)
    //             {
    //                 yield return new TriangleCoord(row, col, pointsUp: false);
    //             }
    //         }
    //     }
    // }
}


// ##########


// var board = new TriangleBoard(5);

// // Enumerate all triangles for rendering
// foreach (var triangleId in board.EnumerateTriangles())
// {
//     var corners = board.GetTriangleCorners(triangleId);
//     RenderTriangle(corners); // Your drawing code
// }

// // Handle click event
// void OnMouseClickVector2
// {
//     var triangleId = board.GetTriangleAtPoint(x, y);
//     if (triangleId.HasValue)
//     {
//         Console.WriteLine($"Clicked on triangle {triangleId.Value}");
//         board.PlacePeg("blue", triangleId.Value);
//     }
// }

// // Place a peg
// public void PlacePeg(string color, TriangleId id)
// {
//     if (triangles.TryGetValue(id, out var triangle))
//     {
//         triangle.Peg = new Peg { Color = color, Triangle = triangle };
//     }
// }

// }