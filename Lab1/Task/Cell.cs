namespace Lab1;

public enum CellType
{
    Free,
    Wall
}

public class Cell
{
    public int? Id { get; set; }
    public CellType Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString()
    {
        if (Type == CellType.Wall) return "#";

        return Id.HasValue ? $"{Id.Value}" : " ";
    }
}