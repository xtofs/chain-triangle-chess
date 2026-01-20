using Models;
using Xunit;

namespace ctc.Tests;

public class DebugBandPlacementTests
{
    private readonly TriangleGeometry _geometry;
    private readonly TriangleBoard _board;

    public DebugBandPlacementTests()
    {
        _geometry = new TriangleGeometry(9, 50, 20, 20);
        _board = new TriangleBoard(_geometry);
    }

    [Fact]
    public void UserBands_ShouldCreatePegAtPosition43()
    {
        // User placed these bands:
        // 4,0/4,3  horizontal
        // 4,1/7,4  diagonal
        // 4,2/7,2  anti-diagonal

        // These bands actually surround triangle (4,3)▽, not (4,4)△
        // Triangle (4,3)▽ has vertices: (4,2), (4,1), (5,2)
        // Its edges are:
        // - (4,2)-(4,1): covered by horizontal band 4,0/4,3
        // - (4,1)-(5,2): covered by diagonal band 4,1/7,4  
        // - (5,2)-(4,2): covered by anti-diagonal band 4,2/7,2

        _board.AddBand(new Vertex(4, 0), new Vertex(4, 3));
        _board.AddBand(new Vertex(4, 1), new Vertex(7, 4));
        _board.AddBand(new Vertex(4, 2), new Vertex(7, 2));

        // Expected: peg at triangle (4, 3)▽
        var expectedTriangle = new Position(4, 3);
        var pegForTriangle = _board.Pegs.FirstOrDefault(p => p.Position == expectedTriangle);

        Assert.NotNull(pegForTriangle);
    }
}
