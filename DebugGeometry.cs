using Models;

var geometry = new TriangleGeometry(9, 50, 20, 20);
var v1 = new Vertex(0, 0);
var v2 = new Vertex(1, 0);

Console.WriteLine($"\nLooking for edge {v1}-{v2}");
Console.WriteLine("All triangles up to row 2:");

int count = 0;
foreach (var tri in geometry.TrianglePositions)
{
    if (tri.Row > 2) break;

    var row = tri.Row;
    var index = tri.Index;
    var col = index / 2;

    Vertex[] vertices;
    if (tri.PointUp)
    {
        vertices = new[]
        {
            new Vertex(row, col),
            new Vertex(row + 1, col),
            new Vertex(row + 1, col + 1)
        };
    }
    else
    {
        vertices = new[]
        {
            new Vertex(row, col + 1),
            new Vertex(row, col),
            new Vertex(row + 1, col + 1)
        };
    }

    var symbol = tri.PointUp ? "△" : "▽";
    Console.WriteLine($"  {tri}{symbol}: {vertices[0]}, {vertices[1]}, {vertices[2]}");

    // Check if both v1 and v2 are in this triangle
    bool hasV1 = vertices.Contains(v1);
    bool hasV2 = vertices.Contains(v2);

    if (hasV1 && hasV2)
    {
        count++;
        Console.WriteLine($"    ^^^ HAS EDGE {v1}-{v2}");
    }
}

Console.WriteLine($"Edge {v1}-{v2} found in {count} triangles");
