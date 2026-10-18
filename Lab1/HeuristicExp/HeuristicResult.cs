namespace Lab1.HeuristicExp;

public sealed record HeuristicResult(
    Dictionary<string, SearchResult> Results,
    Field Field,
    CubeState Start,
    (int x, int y) Target
);