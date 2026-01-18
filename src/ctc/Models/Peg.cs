namespace Models;

public record Peg(int Row, int Col, string Tag = "")
{
    public static implicit operator Peg((int Row, int Col) tuple) => new Peg(tuple.Row, tuple.Col);
    public static implicit operator Peg((int Row, int Col, string Tag) tuple) => new Peg(tuple.Row, tuple.Col, tuple.Tag);

}