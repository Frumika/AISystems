namespace Lab1.NoWayExp;

public class NoWayExperiment
{
    private readonly IReadOnlyList<Algorithm> _algorithms;
    private readonly int _rows;
    private readonly int _columns;
    private readonly Random _random;

    public NoWayExperiment(IReadOnlyList<Algorithm> algorithms, int rows, int columns, Random random)
    {
        _algorithms = algorithms;
        _rows = rows;
        _columns = columns;
        _random = random;
    }

    public NoWayResult Run()
    {
        Field field = Field.CreateClear(_rows, _columns);
        int centerRow = _rows / 2;
        int centerColumn = _columns / 2;

        for (int j = 0; j < _columns; j++)
        {
            field.Cells[centerRow][j].Type = CellType.Wall;
        }

        CubeState start = new(centerColumn, centerRow + 1, Cube.Initial);
        (int x, int y) target = (centerColumn, centerRow - 1);


        Dictionary<string, SearchResult> results = new();
        foreach (var algorithm in _algorithms)
        {
            var result = algorithm.Solve(field, start, target);
            results[algorithm.Name] = result;
        }

        return new NoWayResult(field, start, target, results);
    }
}