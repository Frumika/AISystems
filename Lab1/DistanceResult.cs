namespace Lab1;

public record DistanceResult(
    int Distance,
    IReadOnlyDictionary<string, AlgorithmStats> ByAlgorithm,
    Field ExampleField,
    CubeState Start,
    (int x, int y) Target
);