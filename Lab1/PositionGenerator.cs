namespace Lab1;

public static class PositionGenerator
{
    public static GeneratedPositions Generate(
        Field field, int distance,
        Random random, int maxAttempts = 1000)
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var start = PickStart(field, random);
            var finishes = FindFinishes(field, start, distance);
            if (finishes.Count == 0) continue;

            var finish = finishes[random.Next(finishes.Count)];
            return new GeneratedPositions(start, finish);
        }

        throw new InvalidOperationException(
            $"Не удалось сгенерировать поле с d={distance} за {maxAttempts} попыток " +
            "(поле мало, стен много или для этой грани такого d не существует)."
        );
    }

    private static CubeState PickStart(Field field, Random random)
    {
        int x, y;
        do
        {
            x = random.Next(field.Columns);
            y = random.Next(field.Rows);
        } while (!field.IsWalkable(x, y));

        return new CubeState(x, y, Cube.Initial);
    }

    private static List<(int X, int Y)> FindFinishes(Field field, CubeState start, int distance)
    {
        var visited = new HashSet<CubeState> { start };
        var reachedCells = new HashSet<(int, int)>();
        var finishes = new List<(int X, int Y)>();
        var layer = new List<CubeState> { start };

        for (int depth = 0; depth <= distance && layer.Count > 0; depth++)
        {
            foreach (var state in layer)
            {
                if (state.Cube.Bottom == Face.First && reachedCells.Add((state.X, state.Y)))
                {
                    if (depth == distance && (state.X != start.X || state.Y != start.Y))
                        finishes.Add((state.X, state.Y));
                }
            }

            if (depth == distance) break;

            var next = new List<CubeState>();
            foreach (var state in layer)
            foreach (var direction in Enum.GetValues<MoveDirection>())
            {
                CubeState neighbor = state.Move(direction);
                if (field.IsWalkable(neighbor.X, neighbor.Y) && visited.Add(neighbor))
                    next.Add(neighbor);
            }

            layer = next;
        }

        return finishes;
    }
}