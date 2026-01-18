namespace Models;

public class Triangle(TriangleCoord coord, bool value)
{
    public TriangleCoord Coord { get; } = coord;
    private readonly bool Value = value;
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