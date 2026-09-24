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
    private static readonly List<Cube> TargetCubeOrientations = GetAllCubeOrientations()
        .Where(c => c.Bottom == Face.First)
        .ToList();
    
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
                
                if (queue.Count > maxOpenCount)
                    maxOpenCount = queue.Count;

                if (visited.Count > maxMemoryCount)
                    maxMemoryCount = visited.Count;
            }
        }

        return new SearchResult(null, iterations, maxOpenCount, queue.Count, maxMemoryCount);
    }

    public static SearchResult SolveBidirectional(Field field, CubeState start, (int X, int Y) target)
    {
        var queueForward = new Queue<CubeState>();
        var visitedForward = new HashSet<CubeState> { start };
        var cameFromForward = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();
        queueForward.Enqueue(start);
        
        var queueBackward = new Queue<CubeState>();
        var visitedBackward = new HashSet<CubeState>();
        var cameFromBackward = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();

        foreach (var targetState in GetTargetStates(target))
        {
            queueBackward.Enqueue(targetState);
            visitedBackward.Add(targetState);
        }

        int iterations = 0;
        int maxOpenCount = queueForward.Count + queueBackward.Count;
        int maxMemoryCount = visitedForward.Count + visitedBackward.Count;

        
        while (queueForward.Count > 0 && queueBackward.Count > 0)
        {
            iterations++;
            
            bool expandForward = queueForward.Count <= queueBackward.Count;

            if (expandForward)
            {
                var current = queueForward.Dequeue();

                foreach (var direction in Enum.GetValues<MoveDirection>())
                {
                    var next = current.Move(direction);
                    if (!field.IsWalkable(next.X, next.Y)) continue;
                    if (!visitedForward.Add(next)) continue;
                    
                    int currentMem = visitedForward.Count + visitedBackward.Count;
                    if (currentMem > maxMemoryCount) maxMemoryCount = currentMem;

                    cameFromForward[next] = (current, direction);
                    
                    if (visitedBackward.Contains(next))
                    {
                        int finalOpenCount = queueForward.Count + queueBackward.Count;
                        var path = ReconstructBidirectionalPath(cameFromForward, cameFromBackward, start, next);
                        return new SearchResult(path, iterations, maxOpenCount, finalOpenCount, maxMemoryCount);
                    }

                    queueForward.Enqueue(next);
                    
                    int currentOpen = queueForward.Count + queueBackward.Count;
                    if (currentOpen > maxOpenCount) maxOpenCount = currentOpen;
                }
            }
            else
            {
                var current = queueBackward.Dequeue();

                foreach (var direction in Enum.GetValues<MoveDirection>())
                {
                    var next = current.Move(direction);
                    if (!field.IsWalkable(next.X, next.Y)) continue;
                    if (!visitedBackward.Add(next)) continue;

                    int currentMem = visitedForward.Count + visitedBackward.Count;
                    if (currentMem > maxMemoryCount) maxMemoryCount = currentMem;
                    
                    var forwardMove = Opposite(direction);
                    cameFromBackward[next] = (current, forwardMove);
                    
                    if (visitedForward.Contains(next))
                    {
                        int finalOpenCount = queueForward.Count + queueBackward.Count;
                        var path = ReconstructBidirectionalPath(cameFromForward, cameFromBackward, start, next);
                        return new SearchResult(path, iterations, maxOpenCount, finalOpenCount, maxMemoryCount);
                    }

                    queueBackward.Enqueue(next);

                    int currentOpen = queueForward.Count + queueBackward.Count;
                    if (currentOpen > maxOpenCount) maxOpenCount = currentOpen;
                }
            }
        }

        return new SearchResult(null, iterations, maxOpenCount, queueForward.Count + queueBackward.Count,
            maxMemoryCount);
    }

    private static List<MoveDirection> ReconstructBidirectionalPath(
        Dictionary<CubeState, (CubeState prev, MoveDirection move)> cameFromForward,
        Dictionary<CubeState, (CubeState prev, MoveDirection move)> cameFromBackward,
        CubeState start,
        CubeState meetingState)
    {
        var path = new List<MoveDirection>();

        var forwardPath = new List<MoveDirection>();
        var state = meetingState;
        while (state != start)
        {
            var (prev, move) = cameFromForward[state];
            forwardPath.Add(move);
            state = prev;
        }

        forwardPath.Reverse();
        path.AddRange(forwardPath);

        state = meetingState;
        while (cameFromBackward.TryGetValue(state, out var info))
        {
            path.Add(info.move);
            state = info.prev;
        }

        return path;
    }
    
    private static List<CubeState> GetTargetStates((int X, int Y) target)
    {
        return TargetCubeOrientations
            .Select(cube => new CubeState(target.X, target.Y, cube))
            .ToList();
    }
    
    private static HashSet<Cube> GetAllCubeOrientations()
    {
        var visited = new HashSet<Cube> { Cube.Initial };
        var queue = new Queue<Cube>();
        queue.Enqueue(Cube.Initial);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var dir in Enum.GetValues<MoveDirection>())
            {
                var next = current.Roll(dir);
                if (visited.Add(next))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return visited;
    }
    
    private static MoveDirection Opposite(MoveDirection direction) => direction switch
    {
        MoveDirection.Up => MoveDirection.Down,
        MoveDirection.Down => MoveDirection.Up,
        MoveDirection.Left => MoveDirection.Right,
        MoveDirection.Right => MoveDirection.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

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