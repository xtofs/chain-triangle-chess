using Models;
using ctc.Rendering;
using Xunit;

namespace ctc.Tests;

public class TriangleBoardTests
{
    private readonly TriangleGeometry _geometry;
    private readonly TriangleBoard _board;

    public TriangleBoardTests()
    {
        _geometry = new TriangleGeometry(9, 50, 20, 20);
        var grid = new TriangleGrid(9);
        _board = new TriangleBoard(grid);
    }

    [Fact]
    public void GetTrianglesTouching_ShouldFindEdgeTriangles()
    {
        // Test with an interior edge that's actually shared by 2 triangles
        // Edge (1,0)-(1,1) is shared by triangles (0,0)△ and (1,1)▽
        var v1 = new Vertex(1, 0);
        var v2 = new Vertex(1, 1);

        var touchingTriangles = _board.GetTrianglesTouching(v1, v2).ToList();

        // Each edge of two triangles should have exactly 2 triangles
        Assert.Equal(2, touchingTriangles.Count);
    }

    [Theory]
    [InlineData(1, 0, 1, 1)]  // Horizontal edge - WORKS
    [InlineData(1, 1, 2, 1)]  // Another horizontal edge
    [InlineData(2, 0, 2, 1)]  // Horizontal edge in row 2
    public void GetTrianglesTouching_VariousEdges(int r1, int c1, int r2, int c2)
    {
        var v1 = new Vertex(r1, c1);
        var v2 = new Vertex(r2, c2);

        var touchingTriangles = _board.GetTrianglesTouching(v1, v2).ToList();

        // Each edge should be shared by exactly 2 triangles
        Assert.Equal(2, touchingTriangles.Count);
    }

    [Fact]
    public void AddBand_ShouldStoreBand()
    {
        var v1 = new Vertex(0, 0);
        var v2 = new Vertex(1, 0);

        _board.AddBand(v1, v2);

        Assert.Single(_board.Bands);
        Assert.Equal((v1, v2), _board.Bands[0]);
    }

    [Fact]
    public void AddBand_ShouldNotCreatePegsWithSingleBand()
    {
        var v1 = new Vertex(0, 0);
        var v2 = new Vertex(1, 0);

        _board.AddBand(v1, v2);

        // A triangle needs all 3 edges surrounded, one band touches only 2 triangles
        // and each triangle only has 1 of its 3 edges covered
        Assert.Empty(_board.Pegs);
    }
}
