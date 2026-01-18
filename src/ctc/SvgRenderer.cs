using Models;

public record SvgRenderer(TriangleGeometry geom, string Path)
{


    public void Render((Stud, Stud)[] bands, Peg[] pegs)
    {
        // private static void CreateSvg(TriangleGeometry geom, (Stud, Stud)[] bands, Peg[] pegs, string path)
        // {
        using var writer = File.CreateText(Path);

        writer.WriteLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        // writer.WriteLine("""<!DOCTYPE svg>""");
        writer.WriteLine("""<svg xmlns="http://www.w3.org/2000/svg" width="600" height="500">""");
        writer.WriteLine(STYLE);

        DrawTriangles(writer);

        DrawStuds(writer);

        DrawBands(writer, bands);

        DrawPegs(writer, pegs);

        writer.WriteLine(SCRIPT);

        writer.WriteLine("""</svg>""");
    }

    private void DrawTriangles(StreamWriter writer)
    {
        foreach (var t in geom.TriangleCoords)
        {
            var path = CreateTriangleSvgPath(t);
            var d = t.PointUp ? "up" : "down";

            writer.WriteLine($"""  <path class="tri {d}" d="{path}" onclick='select(evt)'/>""");
        }
    }

    private void DrawBands(StreamWriter writer, (Stud, Stud)[] bands)
    {
        foreach (var band in bands)
        {
            var p = geom.StudToPixel(band.Item1);
            var q = geom.StudToPixel(band.Item2);
            writer.WriteLine($"""    <line class="band" x1="{p.X:f0}" y1="{p.Y:f0}" x2="{q.X:f0}" y2="{q.Y:f0}"/>""");
        }
    }

    private void DrawStuds(StreamWriter writer)
    {
        // draw studs
        foreach (var stud in geom.Studs)
        {
            var px = geom.StudToPixel(stud);
            writer.WriteLine($"""  <circle class="stud" r="4" cx="{px.X:f0}" cy="{px.Y:f0}" onclick='select(evt)'/>""");
        }
    }

    private void DrawPegs(StreamWriter writer, Peg[] pegs)
    {
        // draw pegs
        foreach (var peg in pegs)
        {
            var style = string.IsNullOrEmpty(peg.Tag) ? "" : $"""style="fill:{peg.Tag}" """;
            var px = geom.PegToPixel(peg);
            writer.WriteLine($"""  <circle class="peg" {style} r="10" cx="{px.X:f0}" cy="{px.Y:f0}" onclick='select(evt)'/>""");
            writer.WriteLine($"""  <text class="label" x="{px.X - 7:f0}" y="{px.Y + 2:f0}">{peg.Row},{peg.Col}</text>""");
        }
    }

    private string CreateTriangleSvgPath(TriangleCoord t)
    {
        var corners = geom.GetTriangleCorners(t);
        var path = $"""M {corners[0].X:f0} {corners[0].Y:f0} L {corners[1].X:f0} {corners[1].Y:f0} L {corners[2].X:f0} {corners[2].Y:f0} Z""";
        return path;
    }

    private static readonly string SCRIPT = """
            <script type="text/ecmascript"><![CDATA[
                let selected = undefined
                function select(evt) {
                    var target=evt.target;

                    var next = target == selected ? undefined : target;
                    var prev = selected;
                    
                    prev?.classList.remove('selected');
                    next?.classList.add('selected');

                    selected = next;
                }
                ]]>
            </script>
        """;

    private static readonly string STYLE = """
            <style>
                .stud { fill: white; }
                .stud.selected { fill: hotpink; }
                .tri { }
                .tri.up { fill: #444; }
                .tri.down { fill: #333; }
                .tri.selected { fill: hotpink; }
                .label { pointer-events: none; fill: white; font-size: 8px; font-family: Verdana; }
                .band { stroke: white; stroke-width: 4; }
                .peg { fill: hotpink; }
                svg { background-color: black; }
            </style>
        """;
}