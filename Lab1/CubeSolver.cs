namespace Lab1;

public record SearchResult(
    List<MoveDirection>? Path,
    int Iterations,
    int MaxOpenCount,
    int FinalOpenCount,
    int MaxMemoryCount
);

public static class CubeSolver
{
    public static SearchResult Solve(Field field, CubeState start, (int X, int Y) target)
    {
        var queue = new Queue<CubeState>();
        var visited = new HashSet<CubeState> { start };
        var cameFrom = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();

        queue.Enqueue(start);

        int iterations = 0;
        int maxOpenCount = queue.Count;
        int maxMemoryCount = visited.Count;

        while (queue.Count > 0)
        {
            iterations++;
            var current = queue.Dequeue();

            if (current.X == target.X && current.Y == target.Y && current.Cube.Bottom == Face.First)
            {
                return new SearchResult(
                    ReconstructPath(cameFrom, start, current),
                    iterations,
                    maxOpenCount,
                    queue.Count,
                    maxMemoryCount
                );
            }

            foreach (var direction in Enum.GetValues<MoveDirection>())
            {
                var next = current.Move(direction);
                if (!field.IsWalkable(next.X, next.Y)) continue;
                if (!visited.Add(next)) continue;

                cameFrom[next] = (current, direction);
                queue.Enqueue(next);

                // Обновляем пиковые значения
                if (queue.Count > maxOpenCount)
                    maxOpenCount = queue.Count;

                if (visited.Count > maxMemoryCount)
                    maxMemoryCount = visited.Count;
            }
        }

        return new SearchResult(null, iterations, maxOpenCount, queue.Count, maxMemoryCount);
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