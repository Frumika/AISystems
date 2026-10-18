namespace Lab1;

public class DistanceExperiment
{
    private readonly IReadOnlyList<Algorithm> _algorithms;
    private readonly int _rows;
    private readonly int _columns;
    private readonly int _wallsCount;
    private readonly int _testPerDistance;
    private readonly Random _random;


    public DistanceExperiment(
        IReadOnlyList<Algorithm> algorithms,
        int rows, int columns, int wallsCount, int testPerDistance,
        Random random)
    {
        _algorithms = algorithms;
        _rows = rows;
        _columns = columns;
        _wallsCount = wallsCount;
        _testPerDistance = testPerDistance;
        _random = random;
    }

    public List<DistanceResult> Run(IEnumerable<int> distances) => distances.Select(RunForDistance).ToList();

    public DistanceResult RunForDistance(int distance)
    {
        var tests = GenerateTests(distance);
        var stats = new Dictionary<string, AlgorithmStats>();

        foreach (var algorithm in _algorithms)
        {
            var results = new List<SearchResult>(tests.Count);
            foreach (var (field, positions) in tests)
            {
                var result = algorithm.Solve(field, positions.Start, positions.Finish);
                results.Add(result);
            }

            stats[algorithm.Name] = Aggregate(results, distance);
        }

        return new DistanceResult(distance, stats, tests[0].field, tests[0].positions.Start, tests[0].positions.Finish);
    }


    private List<(Field field, GeneratedPositions positions)> GenerateTests(int distance)
    {
        var tests = new List<(Field field, GeneratedPositions positions)>();

        while (tests.Count < _testPerDistance)
        {
            var field = Field.Create(_rows, _columns, _wallsCount, _random);
            var positions = PositionGenerator.Generate(field, distance, _random, 20);
            tests.Add((field, positions));
        }

        return tests;
    }

    private AlgorithmStats Aggregate(List<SearchResult> results, int distance)
    {
        double avgMaxStored = results.Average(r => r.MaxMemoryCount);

        return new AlgorithmStats(
            AvgSteps: results.Average(r => r.Path!.Count),
            AvgIterations: results.Average(r => r.Iterations),
            AvgMaxOpen: results.Average(r => r.MaxOpenCount),
            AvgFinalOpen: results.Average(r => r.FinalOpenCount),
            AvgMaxStored: avgMaxStored,
            EffectiveBranching: EffectiveBranching.Compute(avgMaxStored, distance));
    }
}