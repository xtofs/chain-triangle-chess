namespace Models;

public record struct Position(int Row, int Index)
{
    public bool PointUp => Index % 2 == 0;
    public override readonly string ToString() => $"({Row},{Index}){(PointUp ? "△" : "▽")}";
}
