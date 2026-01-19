using Models;
using System.IO;

namespace Rendering;

public class TriangleCoordRenderer
{
    private readonly TriangleGeometry _geom;

    public TriangleCoordRenderer(TriangleGeometry geom) => _geom = geom;

    public string CreateTriangleSvgPath(TriangleCoord t)
    {
        var corners = _geom.GetTriangleCorners(t);
        var path = $"M {corners[0].X:f0} {corners[0].Y:f0} L {corners[1].X:f0} {corners[1].Y:f0} L {corners[2].X:f0} {corners[2].Y:f0} Z";
        return path;
    }

    public void Render(StreamWriter writer, TriangleCoord t)
    {
        var path = CreateTriangleSvgPath(t);
        var d = t.PointUp ? "up" : "down";
        writer.WriteLine($"  <path class=\"tri {d}\" d=\"{path}\" onclick='select(evt)'/>");
    }
}
