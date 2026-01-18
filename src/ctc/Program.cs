

using System.Numerics;
using System.Reflection.Emit;
using Models;

internal static class Program
{
    private static void Main(string[] args)
    {
        var geom = new TriangleGeometry(8, 50, 20, 20);


        using var writer = File.CreateText("board.svg");

        writer.WriteLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        // writer.WriteLine("""<!DOCTYPE svg>""");
        writer.WriteLine("""<svg xmlns="http://www.w3.org/2000/svg" width="600" height="500">""");
        writer.WriteLine(STYLE);


        // draw triangles
        foreach (var t in geom.TriangleCoords)
        {
            var path = geom.PathFromTriangle(t);
            var d = t.PointUp ? "up" : "down";

            writer.WriteLine($"""  <path class="tri {d}" d="{path}" onclick='select(evt)'/>""");
        }

        // draw studs
        foreach (var stud in geom.Studs)
        {
            var px = geom.StudToPixel(stud);
            writer.WriteLine($"""  <circle class="stud" r="4" cx="{px.X:f0}" cy="{px.Y:f0}" onclick='select(evt)'/>""");
        }

        var bands = new (Stud, Stud)[]
        {
            ((1,1), (4,4)),
            ((4,2), (7,2)),
            ((7,3), (7,6)),
        };
        foreach (var band in bands)
        {
            var p = geom.StudToPixel(band.Item1);
            var q = geom.StudToPixel(band.Item2);
            writer.WriteLine($"""    <line class="band" x1="{p.X:f0}" y1="{p.Y:f0}" x2="{q.X:f0}" y2="{q.Y:f0}"/>""");
        }

        writer.WriteLine(SCRIPT);

        writer.WriteLine("""</svg>""");
    }

    private static string PathFromTriangle(this TriangleGeometry geom, TriangleCoord t)
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
                .label { pointer-events: none; font-size: 8px; font-family: Verdana; fill: blue; }
                .band { stroke: white; stroke-width: 4; }
                svg { background-color: black; }
            </style>
        """;
}
