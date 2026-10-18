namespace Lab1;

public record SearchResult(
    List<MoveDirection>? Path,
    int Iterations,
    int MaxOpenCount,
    int FinalOpenCount,
    int MaxMemoryCount
);