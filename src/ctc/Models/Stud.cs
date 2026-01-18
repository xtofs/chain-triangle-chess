namespace Models;

public record struct Stud(
     int Row,
     int Col)
{

     public static implicit operator Stud((int Row, int Col) tuple) => new Stud(tuple.Row, tuple.Col);
}
