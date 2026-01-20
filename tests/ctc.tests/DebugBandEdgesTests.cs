using Models;
using ctc.Rendering;
using Xunit;

namespace ctc.Tests;

public class DebugBandEdgesTests
{
    private readonly TriangleGeometry _geometry;
    private readonly TriangleBoard _board;

    public DebugBandEdgesTests()
    {
        _geometry = new TriangleGeometry(9, 50, 20, 20);
        _board = new TriangleBoard(_geometry);
    }

    [Fact]
    public void DebugBandEdges_AntiDiagonalBand()
    {
        // 4,2/7,2 anti-diagonal -> path: (4,2), (5,2), (6,2), (7,2)
        // Creates edges: (4,2)-(5,2), (5,2)-(6,2), (6,2)-(7,2)

        var from = new Vertex(4, 2);
        var to = new Vertex(7, 2);
        var path = _board.GetBandPath(from, to);

        var output = $"Band path {from} to {to}: {string.Join(" -> ", path)}\n";

        for (int i = 0; i < path.Count - 1; i++)
        {
            var v1 = path[i];
            var v2 = path[i + 1];
            var triangles = _board.GetTrianglesTouching(v1, v2).ToList();
            output += $"  Edge {v1}-{v2}: touches {triangles.Count} triangles\n";
            foreach (var tri in triangles)
            {
                output += $"    - {tri}\n";
            }
        }

        System.IO.File.WriteAllText("/tmp/antidiagonal_band_debug.txt", output);
    }

    [Fact]
    public void DebugTriangle44Vertices()
    {
        // What are the three edges of triangle (4,4)?
        var tri = new Position(4, 4);
        var verts = _geometry.GetTriangleVertices(tri);

        var output = $"Triangle {tri}: {verts[0]}, {verts[1]}, {verts[2]}\n";
        output += "Edges:\n";
        for (int i = 0; i < 3; i++)
        {
            var v1 = verts[i];
            var v2 = verts[(i + 1) % 3];
            output += $"  {v1}-{v2}\n";
        }

        System.IO.File.WriteAllText("/tmp/triangle_44_debug.txt", output);
    }
}
