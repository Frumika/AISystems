namespace Lab1;

public abstract class BaseSolver
{
    protected static List<MoveDirection> ReconstructPath(
        Dictionary<CubeState, (CubeState prev, MoveDirection move)> cameFrom,
        CubeState start, CubeState goal)
    {
        var path = new List<MoveDirection>();
        var state = goal;
        while (state != start)
        {
            var (prev, move) = cameFrom[state];
            path.Add(move);
            state = prev;
        }

        path.Reverse();
        return path;
    }
}