namespace Models;


/// <summary>
/// A vertex is a connection point on the triangular board,
/// a corner of a triangle tile,
/// where bands can be attached.
/// </summary>
/// <param name="Row"></param>
/// <param name="Col"></param>
public record struct Vertex(int Row, int Col)
{

     public static implicit operator Vertex((int Row, int Col) tuple) => new Vertex(tuple.Row, tuple.Col);
}
