namespace Lab1;

public static class CubeSolver
{
    public static List<MoveDirection>? Solve(Field field, CubeState start, (int X, int Y) target)
    {
        var queue = new Queue<CubeState>();
        var visited = new HashSet<CubeState> { start };
        var cameFrom = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.X == target.X && current.Y == target.Y && current.Cube.Bottom == Face.First)
                return ReconstructPath(cameFrom, start, current);

            foreach (var direction in Enum.GetValues<MoveDirection>())
            {
                var next = current.Move(direction);
                if (!field.IsWalkable(next.X, next.Y)) continue;
                if (!visited.Add(next)) continue;

                cameFrom[next] = (current, direction);
                queue.Enqueue(next);
            }
        }

        return null;
    }

    private static List<MoveDirection> ReconstructPath(
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