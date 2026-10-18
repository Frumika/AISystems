namespace Lab1.NoWayExp;

public record NoWayResult(
    Field Field,
    CubeState Start,
    (int x, int y) Target,
    Dictionary<string, SearchResult> Results
);