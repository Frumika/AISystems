namespace Lab1.HeuristicExp;

public class HeuristicExperiment
{
    private readonly IReadOnlyList<Algorithm> _algorithms;
    private readonly int _rows;
    private readonly int _columns;

    public HeuristicExperiment(IReadOnlyList<Algorithm> algorithms, int rows, int columns)
    {
        _algorithms = algorithms;
        _rows = rows;
        _columns = columns;
    }

    public HeuristicResult Open()
    {
        Field field = Field.CreateClear(_rows, _columns);


        int startX = 6;
        int startY = _rows - 1;


        int finishX = _columns - 1;
        int finishY = 6;

        CubeState start = new(startX, startY, Cube.Initial);
        (int x, int y) target = (finishX, finishY);

        Dictionary<string, SearchResult> results = new();
        foreach (var algorithm in _algorithms)
        {
            SearchResult result = algorithm.Solve(field, start, target);
            results[algorithm.Name] = result;
        }

        return new HeuristicResult(results, field, start, target);
    }

    public HeuristicResult Trapped()
    {
        Field field = Field.CreateClear(_rows, _columns);

        int centerRow = _rows / 2;

        int startX = _columns / 3;
        int finishX = _columns - _columns / 3;

        CubeState start = new(startX, centerRow, Cube.Initial);
        (int x, int y) target = (finishX, centerRow);


        int upperTrapRow = _rows / 4;
        int lowerTrapRow = _rows - _rows / 4;

        int startTrapColumn = _columns / 4;
        int finishTrapColumn = _columns - _columns / 4;
        int centerTrapColumn = _columns / 2;

        for (int j = startTrapColumn; j <= finishTrapColumn; j++)
        {
            field.Cells[upperTrapRow][j].Type = CellType.Wall;
            field.Cells[lowerTrapRow][j].Type = CellType.Wall;
        }

        for (int i = upperTrapRow; i <= lowerTrapRow; i++)
        {
            if (i != centerRow)
            {
                field.Cells[i][startTrapColumn].Type = CellType.Wall;
                field.Cells[i][finishTrapColumn].Type = CellType.Wall;
            }

            field.Cells[i][centerTrapColumn].Type = CellType.Wall;
        }

        Dictionary<string, SearchResult> results = new();
        foreach (var algorithm in _algorithms)
        {
            SearchResult result = algorithm.Solve(field, start, target);
            results[algorithm.Name] = result;
        }

        return new HeuristicResult(results, field, start, target);
    }
}