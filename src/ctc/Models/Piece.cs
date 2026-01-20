namespace Models;

public record Piece(Position Position, string Color = "white")
{
    // Compatibility accessors
    public int Row => Position.Row;
    public int Index => Position.Index;

    public static implicit operator Piece((int Row, int Index) tuple) => new Piece(new Position(tuple.Row, tuple.Index));
    public static implicit operator Piece((int Row, int Index, string Color) tuple) => new Piece(new Position(tuple.Row, tuple.Index), tuple.Color);
}