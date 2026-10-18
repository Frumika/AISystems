namespace Lab1;

public record Algorithm(
    string Name,
    Func<Field, CubeState, (int X, int Y), SearchResult> Solve
);