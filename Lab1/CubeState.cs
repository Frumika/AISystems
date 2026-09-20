namespace Lab1;

public readonly record struct CubeState(int X, int Y, Cube Cube)
{
    public CubeState Move(MoveDirection direction)
    {
        var (dx, dy) = direction switch
        {
            MoveDirection.Up    => (0, -1),
            MoveDirection.Down  => (0, 1),
            MoveDirection.Left  => (-1, 0),
            MoveDirection.Right => (1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

        return new CubeState(X + dx, Y + dy, Cube.Roll(direction));
    }
}