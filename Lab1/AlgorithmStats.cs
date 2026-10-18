namespace Lab1;

public sealed record AlgorithmStats(
    double AvgIterations,
    double AvgMaxOpen,
    double AvgFinalOpen,
    double AvgMaxStored,
    double EffectiveBranching
);