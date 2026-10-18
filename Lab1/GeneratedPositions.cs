namespace Lab1;

public sealed record GeneratedPositions(
    CubeState Start,
    (int X, int Y) Finish
);