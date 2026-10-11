using Lab1;

var random = Random.Shared;

Field field;
CubeState start;
(int X, int Y) target;
SearchResult resultBfs;
SearchResult resultBidirectional;
SearchResult resultAStart1;
SearchResult resultAStart2;

while (true)
{
    field = Field.Create(10, 12, 12);

    var freeCells = Enumerable.Range(0, field.Rows * field.Columns)
        .Select(i => (x: i % field.Columns, y: i / field.Columns))
        .Where(c => field.IsWalkable(c.x, c.y))
        .OrderBy(_ => random.Next())
        .Take(2)
        .ToArray();

    if (freeCells.Length < 2) continue;

    start = new CubeState(freeCells[0].x, freeCells[0].y, Cube.Initial);
    target = freeCells[1];

    resultBfs = CubeSolver1.Solve(field, start, target);
    resultBidirectional = CubeSolver2.SolveBidirectional(field, start, target);
    resultAStart1 = CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H1);
    resultAStart2 = CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H2);

    if (resultBfs.Path is null) continue;
    if (resultBidirectional.Path is null) continue;
    if (resultAStart1.Path is null) continue;
    if (resultAStart2.Path is null) continue;

    break;
}


Console.WriteLine("==================== BFC ====================");

Console.WriteLine("Начальное поле:");
PrintBoard(field, start, target);

Console.WriteLine($"Решение 1 найдено за {resultBfs.Path.Count} ходов:\n");
PrintSolution(start, resultBfs.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика поиска (BFC) ---");
Console.WriteLine($"1. Количество итераций алгоритма: {resultBfs.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {resultBfs.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {resultBfs.FinalOpenCount}");
Console.WriteLine($"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {resultBfs.MaxMemoryCount}\n");

Console.WriteLine();
Console.WriteLine("-----------------------------------------------");
Console.WriteLine();
Console.WriteLine("==================== Bidirectional ====================");

Console.WriteLine($"Решение 2 найдено за {resultBidirectional.Path.Count} ходов:\n");
PrintSolution(start, resultBidirectional.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика поиска (Bidirectional) ---");
Console.WriteLine($"1. Количество итераций алгоритма: {resultBidirectional.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {resultBidirectional.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {resultBidirectional.FinalOpenCount}");
Console.WriteLine(
    $"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {resultBidirectional.MaxMemoryCount}\n");

Console.WriteLine();
Console.WriteLine("-----------------------------------------------");
Console.WriteLine();
Console.WriteLine("==================== AStar 1 ====================");

Console.WriteLine($"Решение 3.1 найдено за {resultAStart1.Path.Count} ходов:\n");
PrintSolution(start, resultAStart1.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика поиска (AStar) ---");
Console.WriteLine($"1. Количество итераций алгоритма: {resultAStart1.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {resultAStart1.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {resultAStart1.FinalOpenCount}");
Console.WriteLine(
    $"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {resultAStart1.MaxMemoryCount}\n");

Console.WriteLine();
Console.WriteLine("-----------------------------------------------");
Console.WriteLine();
Console.WriteLine("==================== AStar 2 ====================");

Console.WriteLine($"Решение 3.2 найдено за {resultAStart2.Path.Count} ходов:\n");
PrintSolution(start, resultAStart2.Path);

Console.WriteLine();
Console.WriteLine("--- Статистика поиска (AStar 2) ---");
Console.WriteLine($"1. Количество итераций алгоритма: {resultAStart2.Iterations}");
Console.WriteLine($"2. Узлов в списке O:");
Console.WriteLine($"   - Максимальное за время поиска: {resultAStart2.MaxOpenCount}");
Console.WriteLine($"   - На момент завершения: {resultAStart2.FinalOpenCount}");
Console.WriteLine(
    $"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {resultAStart2.MaxMemoryCount}\n");




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