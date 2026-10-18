using Lab1.NoWayExp;

namespace Lab1;

public static class ConsoleHelper
{
    public static void PrintNoWayResults(NoWayResult result)
    {
        Console.WriteLine("НЕВОЗМОЖНО НАЙТИ РЕШЕНИЕ");
        PrintBoard(result.Field, result.Start, result.Target);

        foreach (var (methodName, searchResult) in result.Results)
        {
            string displayText = searchResult.Path is null ? "Не найдено" : "Найдено";
            Console.WriteLine($"Решением методом {methodName} -> {displayText}");
        }
    }


    public static void PrintDistanceResults(List<DistanceResult> results)
    {
        foreach (var result in results)
        {
            PrintDistanceResult(result);
            Console.WriteLine("================================================================");
        }
    }

    private static void PrintDistanceResult(DistanceResult result)
    {
        Console.WriteLine();
        Console.WriteLine($"СТОИМОСТЬ ОПТИМАЛЬНОГО РЕШЕНИЯ (d): {result.Distance}");
        PrintBoard(result.ExampleField, result.Start, result.Target);
        Console.WriteLine();

        foreach (var (methodName, stats) in result.ByAlgorithm)
        {
            Console.WriteLine($"Усредненная статистика поиска ({methodName})");
            Console.WriteLine($"- Решение найдено за {stats.AvgSteps} шагов");
            Console.WriteLine($"- Количество итераций алгоритма: {stats.AvgIterations}");
            Console.WriteLine($"- Узлов в списке O:");
            Console.WriteLine($"-- Максимальное за время поиска: {stats.AvgMaxOpen}");
            Console.WriteLine($"-- На момент завершения: {stats.AvgFinalOpen}");
            Console.WriteLine($"- Максимальное количество хранимых в памяти узлов (|O| + |C|): {stats.AvgMaxStored}");
            Console.WriteLine($"- Коэффициент оптимального ветвления (b*): {stats.EffectiveBranching:F3}");
            Console.WriteLine();
        }
    }


    public static void PrintStatistic(string methodName, SearchResult result, CubeState start, bool solution = true)
    {
        Console.WriteLine();
        Console.WriteLine($"==================== {methodName} ====================");

        Console.WriteLine($"Решение найдено за {result.Path!.Count} ходов:\n");
        if (solution) ConsoleHelper.PrintSolution(start, result.Path);

        Console.WriteLine();
        Console.WriteLine($"--- Статистика поиска ({methodName}) ---");
        Console.WriteLine($"1. Количество итераций алгоритма: {result.Iterations}");
        Console.WriteLine($"2. Узлов в списке O:");
        Console.WriteLine($"   - Максимальное за время поиска: {result.MaxOpenCount}");
        Console.WriteLine($"   - На момент завершения: {result.FinalOpenCount}");
        Console.WriteLine(
            $"3. Максимальное количество хранимых в памяти узлов (|O| + |C|): {result.MaxMemoryCount}\n");
    }

    public static void PrintBoard(Field field, CubeState start, (int X, int Y) target)
    {
        Console.WriteLine("Игровое поле:");

        for (int y = 0; y < field.Rows; y++)
        {
            for (int x = 0; x < field.Columns; x++)
            {
                char symbol = field[x, y].Type == CellType.Wall ? '#'
                    : x == start.X && y == start.Y ? 'С'
                    : x == target.X && y == target.Y ? 'Ф'
                    : '.';

                if (symbol == 'С' || symbol == 'Ф')
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(symbol);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(symbol);
                }

                Console.Write(' ');
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    public static void PrintSolution(CubeState start, IReadOnlyList<MoveDirection> moves)
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

    private static string TranslateFace(Face face)
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

    private static string TranslateDirection(MoveDirection direction)
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
}