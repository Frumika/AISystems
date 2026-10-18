namespace Lab1;

public class CubeSolver2 : BaseSolver
{
    private static readonly List<Cube> TargetCubeOrientations = GetAllCubeOrientations()
        .Where(c => c.Bottom == Face.First)
        .ToList();

    public static SearchResult SolveBidirectional(Field field, CubeState start, (int X, int Y) target)
    {
        var distForward = new Dictionary<CubeState, int> { [start] = 0 };
        var cameFromForward = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();
        var queueForward = new Queue<CubeState>();
        queueForward.Enqueue(start);

        var distBackward = new Dictionary<CubeState, int>();
        var cameFromBackward = new Dictionary<CubeState, (CubeState prev, MoveDirection move)>();
        var queueBackward = new Queue<CubeState>();

        foreach (var targetState in GetTargetStates(target))
        {
            if (distBackward.TryAdd(targetState, 0))
                queueBackward.Enqueue(targetState);
        }

        int totalProcessedNodes = 0;
        int maxOpenCount = queueForward.Count + queueBackward.Count;
        int maxMemoryCount = distForward.Count + distBackward.Count;

        int forwardDepth = 0;
        int backwardDepth = 0;

        bool found = false;
        int bestTotal = 0;
        CubeState bestMeeting = default;

        while (queueForward.Count > 0 && queueBackward.Count > 0)
        {
            if (found) break;
            
            List<(CubeState Node, int Total)> matches;
            int processedInThisLayer = 0;

            if (forwardDepth <= backwardDepth)
            {
                forwardDepth++;
                processedInThisLayer = queueForward.Count;
                matches = ExpandLayer(queueForward, distForward, distBackward, cameFromForward, field,
                    direction => direction, forwardDepth);
            }
            else
            {
                backwardDepth++;
                processedInThisLayer = queueBackward.Count;
                matches = ExpandLayer(queueBackward, distBackward, distForward, cameFromBackward, field, Opposite,
                    backwardDepth);
            }
            
            totalProcessedNodes += processedInThisLayer;

            foreach (var (node, total) in matches)
            {
                if (!found || total < bestTotal)
                {
                    found = true;
                    bestTotal = total;
                    bestMeeting = node;
                }
            }

            int currentMem = distForward.Count + distBackward.Count;
            if (currentMem > maxMemoryCount) maxMemoryCount = currentMem;

            int currentOpen = queueForward.Count + queueBackward.Count;
            if (currentOpen > maxOpenCount) maxOpenCount = currentOpen;
        }

        int finalOpenCount = queueForward.Count + queueBackward.Count;

        if (!found)
            return new SearchResult(null, totalProcessedNodes, maxOpenCount, finalOpenCount, maxMemoryCount);

        var path = ReconstructBidirectionalPath(cameFromForward, cameFromBackward, start, bestMeeting);
        return new SearchResult(path, totalProcessedNodes, maxOpenCount, finalOpenCount, maxMemoryCount);
    }

    private static List<(CubeState Node, int Total)> ExpandLayer(
        Queue<CubeState> queue,
        Dictionary<CubeState, int> distOwn,
        Dictionary<CubeState, int> distOther,
        Dictionary<CubeState, (CubeState prev, MoveDirection move)> cameFrom,
        Field field,
        Func<MoveDirection, MoveDirection> moveTransform,
        int newDepth)
    {
        var matches = new List<(CubeState, int)>();
        int layerSize = queue.Count;

        for (int i = 0; i < layerSize; i++)
        {
            var current = queue.Dequeue();

            foreach (var direction in Enum.GetValues<MoveDirection>())
            {
                var next = current.Move(direction);
                if (!field.IsWalkable(next.X, next.Y)) continue;
                if (!distOwn.TryAdd(next, newDepth)) continue;

                cameFrom[next] = (current, moveTransform(direction));
                queue.Enqueue(next);

                if (distOther.TryGetValue(next, out var otherDepth))
                    matches.Add((next, newDepth + otherDepth));
            }
        }

        return matches;
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
}