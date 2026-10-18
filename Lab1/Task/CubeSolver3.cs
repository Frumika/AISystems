namespace Lab1;

public class CubeSolver3 : BaseSolver
{
    public static int H1(CubeState state, (int X, int Y) target)
    {
        return Math.Abs(state.X - target.X) + Math.Abs(state.Y - target.Y);
    }

    public static int H2(CubeState state, (int X, int Y) target)
    {
        int h1 = H1(state, target);

        int orientationCost = 0;
        if (state.Cube.Bottom != Face.First)
        {
            orientationCost = state.Cube.Top == Face.First ? 2 : 1;
        }

        if (h1 == 0 && orientationCost > 0)
        {
            return 2;
        }

        return Math.Max(h1, orientationCost);
    }

    public static SearchResult SolveAStar(
        Field field,
        CubeState start,
        (int X, int Y) target,
        Func<CubeState, (int X, int Y), int> heuristic)
    {
        var gCosts = new Dictionary<CubeState, int> { [start] = 0 };
        var cameFrom = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();
        var closed = new HashSet<CubeState>();
        var openQueue = new PriorityQueue<CubeState, int>();

        openQueue.Enqueue(start, heuristic(start, target));

        int iterations = 0;
        int maxOpenCount = 1;

        while (openQueue.Count > 0)
        {
            var current = openQueue.Dequeue();
            if (!closed.Add(current)) continue;

            iterations++;

            if (current.X == target.X && current.Y == target.Y && current.Cube.Bottom == Face.First)
            {
                return new SearchResult(
                    ReconstructPath(cameFrom, start, current),
                    iterations,
                    maxOpenCount,
                    gCosts.Count - closed.Count,
                    gCosts.Count);
            }

            int currentG = gCosts[current];

            foreach (var direction in Enum.GetValues<MoveDirection>())
            {
                var next = current.Move(direction);
                if (!field.IsWalkable(next.X, next.Y)) continue;

                int tentativeG = currentG + 1;
                if (gCosts.TryGetValue(next, out int existingG) && tentativeG >= existingG)
                    continue;

                gCosts[next] = tentativeG;
                cameFrom[next] = (current, direction);
                closed.Remove(next);

                openQueue.Enqueue(next, tentativeG + heuristic(next, target));

                int openCount = gCosts.Count - closed.Count;
                if (openCount > maxOpenCount) maxOpenCount = openCount;
            }
        }

        return new SearchResult(null, iterations, maxOpenCount, 0, gCosts.Count);
    }
}