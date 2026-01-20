using Models;

public record SvgRenderer(TriangleGeometry Geometry)
{
    public async Task RenderStatic(Stream output)
    {
        using var writer = new StreamWriter(output, leaveOpen: true);

        DrawTriangles(writer);

        DrawVertices(writer);

        await writer.FlushAsync();
    }

    public async Task RenderDynamic(Stream output, TriangleBoard game)
    {
        using var writer = new StreamWriter(output, leaveOpen: true);

        DrawBands(writer, game.Bands);

        DrawPegs(writer, game.Pegs);

        await writer.FlushAsync();
    }

    private void DrawTriangles(StreamWriter writer)
    {
        foreach (var t in Geometry.TrianglePositions)
        {
            var path = CreateTriangleSvgPath(t);
            var d = t.PointUp ? "up" : "down";

            writer.WriteLine($"""    <path class="tri {d}" d="{path}"/>""");

#if ALL_LABELS
            var center = Geometry.GetTriangleCenter(t);
            var label = $"{(char)('a' + t.Index / 2)}{t.Row}";
            writer.WriteLine($"""    <text class="label" x="{center.X:f0}" y="{center.Y:f0}">{label}</text>""");
#endif
        }
    }

    private void DrawBands(StreamWriter writer, (Vertex, Vertex)[] bands)
    {
        foreach (var band in bands)
        {
            var p = Geometry.VertexToPixel(band.Item1);
            var q = Geometry.VertexToPixel(band.Item2);
            writer.WriteLine($"""    <line class="band" x1="{p.X:f0}" y1="{p.Y:f0}" x2="{q.X:f0}" y2="{q.Y:f0}"/>""");
        }
    }

    /// <summary>
    /// Draw all Vertices on the board.
    /// </summary>
    private void DrawVertices(StreamWriter writer)
    {
        foreach (var vertex in GetVertices(Geometry.Size))
        {
            var px = Geometry.VertexToPixel(vertex);
            writer.WriteLine($"""    <circle class="vertex" r="4" cx="{px.X:f0}" cy="{px.Y:f0}"/>""");
            // Invisible clickable hit zone (larger than visible vertex)
            writer.WriteLine($"""    <circle class="vertex-hit" r="16" cx="{px.X:f0}" cy="{px.Y:f0}" hx-post="/api/select/{vertex.Row},{vertex.Col}" hx-swap="none"/>""");
        }
    }

    static IEnumerable<Vertex> GetVertices(int size)
    {
        for (int row = 0; row < size + 1; row++)
        {
            for (int col = 0; col <= row; col++)
            {
                yield return new Vertex(row, col);
            }
        }
    }

    private void DrawPegs(StreamWriter writer, Piece[] pegs)
    {
        foreach (var peg in pegs)
        {
            var style = string.IsNullOrEmpty(peg.Tag) ? "" : $"""style="fill:{peg.Tag}" """;
            var px = Geometry.GetTriangleCenter(peg.Position);
            writer.WriteLine($"""    <circle class="peg" {style} r="10" cx="{px.X:f0}" cy="{px.Y:f0}" onclick="select(evt)"/>""");

            var txt = $"{Alpha(peg.Position.Index)}{peg.Position.Row}";
            writer.WriteLine($"""    <text class="label" x="{px.X:f0}" y="{px.Y:f0}" >{txt}</text>""");
        }

        static char Alpha(int v) => (char)(v + 'a');
    }

    private string CreateTriangleSvgPath(Position pos)
    {
        var corners = Geometry.GetTriangleCorners(pos);
        var path = $"M {corners[0].X:f0} {corners[0].Y:f0} L {corners[1].X:f0} {corners[1].Y:f0} L {corners[2].X:f0} {corners[2].Y:f0} Z";
        return path;
    }
}