namespace Lab1;

public readonly record struct Cube(
    Face Top,
    Face Bottom,
    Face North,
    Face South,
    Face East,
    Face West)
{
    public static Cube Initial => new(
        Top: Face.First, Bottom: Face.Second,
        North: Face.Third, South: Face.Fourth,
        East: Face.Fifth, West: Face.Sixth);

    public Cube Roll(MoveDirection direction) => direction switch
    {
        MoveDirection.Right => this with { Top = West, East = Top, Bottom = East, West = Bottom },
        MoveDirection.Left => this with { Top = East, West = Top, Bottom = West, East = Bottom },
        MoveDirection.Down => this with { Top = North, South = Top, Bottom = South, North = Bottom },
        MoveDirection.Up => this with { Top = South, North = Top, Bottom = North, South = Bottom },
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}