namespace Models;

using System.Collections.Generic;
using System.Numerics;

public class TriangleBoard
{
    private readonly int _size;

    private Dictionary<TriangleCoord, Triangle> triangles = new();

    public TriangleGeometry Geometry { get; }

    public TriangleBoard(int rows)
    {
        _size = rows;
        // InitializeStuds(rows);
        // ConnectStuds();
        // CreateTriangles();
    }

    public TriangleBoard(TriangleGeometry geom)
    {
        Geometry = geom;
        _size = geom.Size;
    }

    // ... previous stud initialization code ...

    // private void CreateTriangles()
    // {
    //     // foreach (var key in studs.Keys)
    //     // {
    //     //     var (row, col) = key;
    //     //     var stud = studs[key].Value;

    //     //     // Point-up triangle: apex at current stud
    //     //     if (stud.DownLeft != null && stud.DownRight != null)
    //     //     {
    //     //         var id = new TriangleCoord(row, col, pointsUp: true);
    //     //         triangles[id] = new Triangle
    //     //         {
    //     //             Id = id,
    //     //             Studs = new[] { stud, stud.DownLeft, stud.DownRight }
    //     //         };
    //     //     }

    //     //     // Point-down triangle: upper-left at current stud
    //     //     if (stud.Right != null && stud.DownRight != null)
    //     //     {
    //     //         var id = new TriangleCoord(row, col, pointsUp: false);
    //     //         triangles[id] = new Triangle
    //     //         {
    //     //             Id = id,
    //     //             Studs = new[] { stud, stud.Right, stud.DownRight }
    //     //         };
    //     //     }
    // }
    // }

    // // a) Get pixel coordinates for triangle corners from identifier
    // public Vector2[] GetTriangleCorners(TriangleCoord id)
    // {
    //     if (!triangles.TryGetValue(id, out var triangle))
    //         return null;

    //     return triangle.Studs
    //         .Select(s => Geometry.StudToPixel(s.Row, s.Col))
    //         .ToArray();
    // }

    // // b) Get triangle coordinates from pixel coordinates (for click events)
    // public TriangleCoord? GetTriangleAtPoint(Vector2 point)
    // {
    //     // First, find the nearest stud
    //     var (row, col) = Geometry.PixelToStud(point);

    //     // Get candidate triangles near this point
    //     var candidates = GetTrianglesNear(row, col);

    //     // Test which triangle actually contains the point
    //     foreach (var candidate in candidates)
    //     {
    //         if (IsPointInTriangle(x, y, candidate))
    //             return candidate.Id;
    //     }

    //     return null;
    // }

    // private List<Triangle> GetTrianglesNear(int row, int col)
    // {
    //     var candidates = new List<Triangle>();

    //     // Check up to 6 possible triangles around this stud
    //     for (int r = Math.Max(0, row - 1); r <= Math.Min(_size - 1, row); r++)
    //     {
    //         for (int c = Math.Max(0, col - 1); c <= Math.Min(r, col); c++)
    //         {
    //             var upId = new TriangleCoord(r, c, true);
    //             if (triangles.TryGetValue(upId, out var upTri))
    //                 candidates.Add(upTri);

    //             var downId = new TriangleCoord(r, c, false);
    //             if (triangles.TryGetValue(downId, out var downTri))
    //                 candidates.Add(downTri);
    //         }
    //     }

    //     return candidates;
    // }

    // private bool IsPointInTriangle(float px, float py, Triangle triangle)
    // {
    //     var corners = triangle.Studs
    //         .Select(s => Geometry.StudToPixel(s.Row, s.Col))
    //         .ToArray();

    //     // Use barycentric coordinates or cross-product method
    //     return IsPointInTriangle(px, py, corners[0], corners[1], corners[2]);
    // }

    // private static bool IsPointInTriangle(float px, float py,
    //     Vector2 p0, Vector2 p1, Vector2 p2)
    // {
    //     // Calculate barycentric coordinates
    //     float denominator = (p1.y - p2.y) * (p0.x - p2.x) + (p2.x - p1.x) * (p0.y - p2.y);
    //     float a = ((p1.y - p2.y) * (px - p2.x) + (p2.x - p1.x) * (py - p2.y)) / denominator;
    //     float b = ((p2.y - p0.y) * (px - p2.x) + (p0.x - p2.x) * (py - p2.y)) / denominator;
    //     float c = 1 - a - b;

    //     return a >= 0 && a <= 1 && b >= 0 && b <= 1 && c >= 0 && c <= 1;
    // }

    // // Enumerate all triangle identifiers for grid of size N
    // public IEnumerable<TriangleCoord> EnumerateTriangles()
    // {
    //     return triangles.Keys;
    // }

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
        var triangle = PegToTriangleCoord(peg);
        return Geometry!.GetTriangleCenter(triangle);
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