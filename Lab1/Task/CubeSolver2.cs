namespace Lab1;

public class CubeSolver2 : BaseSolver
{
    private static readonly List<Cube> TargetCubeOrientations = GetAllCubeOrientations()
        .Where(c => c.Bottom == Face.First)
        .ToList();

    private static readonly MoveDirection[] AllDirections = Enum.GetValues<MoveDirection>();

    private sealed class SearchSide
    {
        public Dictionary<CubeState, int> Dist { get; } = new();
        public Dictionary<CubeState, (CubeState prev, MoveDirection move)> CameFrom { get; } = new();
        public Queue<CubeState> Queue { get; } = new();

        public void AddRoot(CubeState state)
        {
            if (Dist.TryAdd(state, 0))
                Queue.Enqueue(state);
        }
    }

    public static SearchResult SolveBidirectional(Field field, CubeState start, (int X, int Y) target)
    {
        var forward = new SearchSide();
        var backward = new SearchSide();

        forward.AddRoot(start);
        foreach (var targetState in GetTargetStates(target))
            backward.AddRoot(targetState);

        int iterations = 0;
        int maxOpenCount = forward.Queue.Count + backward.Queue.Count;
        int maxMemoryCount = forward.Dist.Count + backward.Dist.Count;

        CubeState? meeting = backward.Dist.ContainsKey(start) ? start : null;

        while (meeting is null && forward.Queue.Count > 0 && backward.Queue.Count > 0)
        {
            bool forwardTurn = forward.Queue.Count <= backward.Queue.Count;
            var own = forwardTurn ? forward : backward;
            var other = forwardTurn ? backward : forward;

            int layerSize = own.Queue.Count;

            for (int i = 0; i < layerSize && meeting is null; i++)
            {
                var current = own.Queue.Dequeue();
                iterations++;

                foreach (var direction in AllDirections)
                {
                    var next = current.Move(direction);
                    if (!field.IsWalkable(next.X, next.Y) || own.Dist.ContainsKey(next))
                        continue;

                    own.Dist[next] = own.Dist[current] + 1;
                    own.CameFrom[next] = (current, forwardTurn ? direction : Opposite(direction));
                    own.Queue.Enqueue(next);

                    if (other.Dist.ContainsKey(next))
                    {
                        meeting = next;
                        break;
                    }
                }

                int open = forward.Queue.Count + backward.Queue.Count;
                if (open > maxOpenCount) maxOpenCount = open;

                int memory = forward.Dist.Count + backward.Dist.Count;
                if (memory > maxMemoryCount) maxMemoryCount = memory;
            }
        }

        int finalOpenCount = forward.Queue.Count + backward.Queue.Count;

        if (meeting is null)
            return new SearchResult(null, iterations, maxOpenCount, finalOpenCount, maxMemoryCount);

        var path = ReconstructBidirectionalPath(forward.CameFrom, backward.CameFrom, start, meeting.Value);
        return new SearchResult(path, iterations, maxOpenCount, finalOpenCount, maxMemoryCount);
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