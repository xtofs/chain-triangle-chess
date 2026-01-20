using System.Diagnostics.CodeAnalysis;

namespace Models;


/// <summary>
/// A vertex is a connection point on the triangular board,
/// a corner of a triangle tile,
/// where bands can be attached.
/// </summary>
/// <param name="Row"></param>
/// <param name="Col"></param>
public record struct Vertex(int Row, int Col) : IParsable<Vertex>
{
     public static Vertex Parse(string s, IFormatProvider? provider)
     {
          return TryParse(s, provider, out var result)
               ? result
               : throw new FormatException($"Cannot parse '{s}' as Vertex");
     }

     public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Vertex result)
     {
          var parts = s?.Split(',');
          if (parts?.Length == 2 && int.TryParse(parts[0], out int row) && int.TryParse(parts[1], out int col))
          {
               result = new Vertex(row, col);
               return true;
          }
          result = default;
          return false;
     }

     public static implicit operator Vertex((int Row, int Col) tuple) => new Vertex(tuple.Row, tuple.Col);
}
