namespace Lab1;

public class Field
{
    public Cell[][] Cells { get; }
    public int Rows { get; }
    public int Columns { get; }
    public (int x, int y)[] FreeCells => GetFreeCells();

    private Field(Cell[][] cells, int rows, int columns)
    {
        Cells = cells;
        Rows = rows;
        Columns = columns;
    }

    public Cell this[int x, int y] => Cells[y][x];

    public bool InBounds(int x, int y) => x >= 0 && x < Columns && y >= 0 && y < Rows;

    public bool IsWalkable(int x, int y) => InBounds(x, y) && Cells[y][x].Type == CellType.Free;


    public static Field CreateClear(int rows, int columns)
    {
        var cells = new Cell[rows][];
        for (int y = 0; y < rows; y++)
        {
            cells[y] = new Cell[columns];
            for (int x = 0; x < columns; x++)
                cells[y][x] = new Cell
                {
                    X = x,
                    Y = y,
                    Type = CellType.Free
                };
        }

        return new Field(cells, rows, columns);
    }

    public static Field Create(int rows, int columns, int wallsCount, Random random)
    {
        var cells = new Cell[rows][];
        for (int y = 0; y < rows; y++)
        {
            cells[y] = new Cell[columns];
            for (int x = 0; x < columns; x++)
                cells[y][x] = new Cell
                {
                    X = x,
                    Y = y,
                    Type = CellType.Free
                };
        }

        var freeCoords = Enumerable.Range(0, rows * columns)
            .Select(i => (x: i % columns, y: i / columns))
            .OrderBy(_ => random.Next())
            .Take(wallsCount);

        foreach (var (x, y) in freeCoords)
            cells[y][x].Type = CellType.Wall;

        return new Field(cells, rows, columns);
    }

    private (int x, int y)[] GetFreeCells()
    {
        return Enumerable.Range(0, Rows * Columns)
            .Select(i => (x: i % Columns, y: i / Columns))
            .Where(c => IsWalkable(c.x, c.y))
            .ToArray();
    }
}