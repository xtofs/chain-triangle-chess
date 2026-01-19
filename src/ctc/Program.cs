

using System.Numerics;
using System.Reflection.Emit;
using Models;

internal static class Program
{
    private static void Main(string[] args)
    {
        var geom = new TriangleGeometry(9, 50, 20, 20);
        var renderer = new SvgRenderer(geom, "board.svg");

        var bands = new (Stud, Stud)[]
        {
            ((1,1), (4,4)),
            ((4,2), (7,2)),
            ((6,1), (6,4)),
            ((5,0), (8,3)),
        };

        var pegs = new Peg[]
        {
            (6, 3, "red"),

            (0, 0, "hotpink"),
            (8, 0, "hotpink"),
            (8, 16, "hotpink"),

            // (5, 2, "blue"),
            // (7, 4, "green"),
            // (4, 1, "orange"),
        };
        var game = new TriangleChessGame(bands, pegs);

        renderer.Render(game);
        Console.WriteLine($"Wrote svg file to {renderer.Path}");
    }
}
