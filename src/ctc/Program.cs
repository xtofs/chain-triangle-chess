

using System.Numerics;
using System.Reflection.Emit;
using Models;

internal static class Program
{
    private static void Main(string[] args)
    {
        var geom = new TriangleGeometry(9, 50, 20, 20);
        // var board = new TriangleBoard(geom.Size);
        var renderer = new SvgRenderer(geom, "board.svg");

        var bands = new (Vertex, Vertex)[]
        {
            ((1,1), (4,4)),
            ((4,2), (7,2)),
            ((6,1), (6,4)),
            ((5,0), (8,3)),
        };

        var pegs = new Piece[]
        {
            (6, 3, "red"),

            // to validate the corners of the board
            // (0, 0, "hotpink"),
            // (8, 0, "hotpink"),
            // (8, 16, "hotpink"),
        };
        var game = new TriangleBoard(bands, pegs);

        renderer.Render(game);
        Console.WriteLine($"Wrote svg file to {renderer.Path}");
    }
}
