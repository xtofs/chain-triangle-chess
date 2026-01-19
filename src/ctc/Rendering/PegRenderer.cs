using Models;
using System.IO;

namespace Rendering;

public class PegRenderer
{
    private readonly TriangleGeometry _geom;

    public PegRenderer(TriangleGeometry geom) => _geom = geom;

    public void Render(StreamWriter writer, Peg peg)
    {
        var style = string.IsNullOrEmpty(peg.Tag) ? "" : $"style=\"fill:{peg.Tag}\" ";
        var px = _geom.PegToPixel(peg);
        writer.WriteLine($"  <circle class=\"peg\" {style} r=\"10\" cx=\"{px.X:f0}\" cy=\"{px.Y:f0}\" onclick='select(evt)'/>");
        writer.WriteLine($"  <text class=\"label\" x=\"{px.X - 7:f0}\" y=\"{px.Y + 2:f0}\">{peg.Row},{peg.Col}</text>");
    }
}
