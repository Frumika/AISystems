namespace Lab1;

public sealed record AlgorithmStats(
    double AvgSteps,
    double AvgIterations,
    double AvgMaxOpen,
    double AvgFinalOpen,
    double AvgMaxStored,
    double EffectiveBranching
);