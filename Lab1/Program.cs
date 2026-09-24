using Lab1;

var random = Random.Shared;

Field field;
CubeState start;
(int X, int Y) target;
SearchResult result1;
SearchResult result2;

while (true)
{
    field = Field.Create(6, 8, 8);

    var freeCells = Enumerable.Range(0, field.Rows * field.Columns)
        .Select(i => (x: i % field.Columns, y: i / field.Columns))
        .Where(c => field.IsWalkable(c.x, c.y))
        .OrderBy(_ => random.Next())
        .Take(2)
        .ToArray();

    if (freeCells.Length < 2) continue;

    start = new CubeState(freeCells[0].x, freeCells[0].y, Cube.Initial);
    target = freeCells[1];

    result1 = CubeSolver.Solve(field, start, target);
    result2 = CubeSolver.SolveBidirectional(field, start, target);
    if (result1.Path is null) continue;
    if (result2.Path is null) continue;

    break;
}

Console.WriteLine("Начальное поле:");
PrintBoard(field, start, target);

Console.WriteLine($"Решение 1 найдено за {result1.Path.Count} ходов:\n");
PrintSolution(start, result1.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика 1 поиска ---");
Console.WriteLine($"1. Количество итераций алгоритма: {result1.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {result1.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {result1.FinalOpenCount}");
Console.WriteLine($"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {result1.MaxMemoryCount}\n");

Console.WriteLine();
Console.WriteLine("-----------------------------------------------");
Console.WriteLine();

Console.WriteLine($"Решение 2 найдено за {result2.Path.Count} ходов:\n");
PrintSolution(start, result2.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика 2 поиска ---");
Console.WriteLine($"1. Количество итераций алгоритма: {result2.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {result2.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {result2.FinalOpenCount}");
Console.WriteLine($"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {result2.MaxMemoryCount}\n");


return;

static void PrintBoard(Field field, CubeState start, (int X, int Y) target)
{
    for (int y = 0; y < field.Rows; y++)
    {
        for (int x = 0; x < field.Columns; x++)
        {
            char symbol = field[x, y].Type == CellType.Wall ? '#'
                : x == start.X && y == start.Y ? 'С'
                : x == target.X && y == target.Y ? 'Ф'
                : '.';

            Console.Write(symbol);
            Console.Write(' ');
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

static void PrintSolution(CubeState start, IReadOnlyList<MoveDirection> moves)
{
    var state = start;
    Console.WriteLine($"Старт:  позиция ({state.X}, {state.Y}), низ = {TranslateFace(state.Cube.Bottom)}");

    for (int i = 0; i < moves.Count; i++)
    {
        state = state.Move(moves[i]);
        Console.WriteLine(
            $"Шаг {i + 1,2}: {TranslateDirection(moves[i]),-5} → позиция ({state.X}, {state.Y}), низ = {TranslateFace(state.Cube.Bottom)}"
        );
    }
}

static string TranslateFace(Face face)
{
    return face switch
    {
        Face.First => "Первая (красная)",
        Face.Second => "Вторая",
        Face.Third => "Третья",
        Face.Fourth => "Четвёртая",
        Face.Fifth => "Пятая",
        Face.Sixth => "Шестая",
        _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
    };
}

static string TranslateDirection(MoveDirection direction)
{
    return direction switch
    {
        MoveDirection.Up => "Вверх",
        MoveDirection.Down => "Вниз",
        MoveDirection.Left => "Влево",
        MoveDirection.Right => "Вправо",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}