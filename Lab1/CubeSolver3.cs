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
        // g(n)
        var gCosts = new Dictionary<CubeState, int> { [start] = 0 };
        var cameFrom = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();

        var openQueue = new PriorityQueue<CubeState, int>();

        int hStart = heuristic(start, target);
        openQueue.Enqueue(start, hStart);

        int iterations = 0;
        int maxOpenCount = openQueue.Count;
        int maxMemoryCount = gCosts.Count;

        while (openQueue.Count > 0)
        {
            iterations++;
            var current = openQueue.Dequeue();

            if (current.X == target.X && current.Y == target.Y && current.Cube.Bottom == Face.First)
            {
                return new SearchResult(
                    ReconstructPath(cameFrom, start, current),
                    iterations,
                    maxOpenCount,
                    openQueue.Count,
                    maxMemoryCount
                );
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

                int h = heuristic(next, target);
                int f = tentativeG + h;

                openQueue.Enqueue(next, f);

                if (openQueue.Count > maxOpenCount)
                    maxOpenCount = openQueue.Count;

                if (gCosts.Count > maxMemoryCount)
                    maxMemoryCount = gCosts.Count;
            }
        }

        return new SearchResult(null, iterations, maxOpenCount, openQueue.Count, gCosts.Count);
    }
}