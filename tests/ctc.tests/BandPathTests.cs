using Models;
using Xunit;

namespace ctc.Tests;

public class BandPathTests
{
    private readonly TriangleGeometry _geometry;
    private readonly TriangleBoard _board;

    public BandPathTests()
    {
        _geometry = new TriangleGeometry(9, 50, 20, 20);
        _board = new TriangleBoard(_geometry);
    }

    [Fact]
    public void BandPath_Horizontal()
    {
        // 4,0/4,3 horizontal -> 4,0  4,1  4,2  4,3
        var from = new Vertex(4, 0);
        var to = new Vertex(4, 3);

        var path = _board.GetBandPath(from, to);

        Assert.Equal(4, path.Count);
        Assert.Equal(from, path[0]);
        Assert.Equal(new Vertex(4, 1), path[1]);
        Assert.Equal(new Vertex(4, 2), path[2]);
        Assert.Equal(to, path[3]);
    }

    [Fact]
    public void BandPath_Diagonal()
    {
        // 4,1/7,4 diagonal -> 4,1  5,2  6,3  7,4
        var from = new Vertex(4, 1);
        var to = new Vertex(7, 4);

        var path = _board.GetBandPath(from, to);

        Assert.Equal(4, path.Count);
        Assert.Equal(from, path[0]);
        Assert.Equal(new Vertex(5, 2), path[1]);
        Assert.Equal(new Vertex(6, 3), path[2]);
        Assert.Equal(to, path[3]);
    }

    [Fact]
    public void BandPath_AntiDiagonal()
    {
        // 4,2/7,2 anti-diagonal -> 4,2  5,2  6,2  7,2
        var from = new Vertex(4, 2);
        var to = new Vertex(7, 2);

        var path = _board.GetBandPath(from, to);

        Assert.Equal(4, path.Count);
        Assert.Equal(from, path[0]);
        Assert.Equal(new Vertex(5, 2), path[1]);
        Assert.Equal(new Vertex(6, 2), path[2]);
        Assert.Equal(to, path[3]);
    }

    [Fact]
    public void BandPath_EdgesAreValid()
    {
        // Each edge in the band path should touch exactly 2 triangles
        var from = new Vertex(4, 2);
        var to = new Vertex(7, 2);

        var path = _board.GetBandPath(from, to);

        // Verify each edge touches 2 triangles
        for (int i = 0; i < path.Count - 1; i++)
        {
            var v1 = path[i];
            var v2 = path[i + 1];
            var touchingTriangles = _board.GetTrianglesTouching(v1, v2).ToList();
            Assert.Equal(2, touchingTriangles.Count);
        }
    }
}