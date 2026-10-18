using Lab1;
using Lab1.NoWayExp;


Random random = new Random(12345678);
const int rows = 24;
const int columns = 34;
const int wallsCount = 95;

List<Algorithm> algorithms = new List<Algorithm>
{
    new("BFC", CubeSolver1.Solve),
    new("Bidirectional", CubeSolver2.SolveBidirectional),
    new("AStar1", (field, start, target) => CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H1)),
    new("AStar2", (field, start, target) => CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H2))
};

DistanceExperiment distanceExperiment = new(algorithms, rows, columns, wallsCount, 20, random);
List<DistanceResult> distanceResults = distanceExperiment.Run(new[] { 2, 7, 12, 17, 22 });
ConsoleHelper.PrintDistanceResults(distanceResults);

Console.WriteLine();
Console.WriteLine("=====================================================================");
Console.WriteLine("=====================================================================");
Console.WriteLine();

NoWayExperiment noWayExperiment = new(algorithms, 10, 10, random);
var noWayResult = noWayExperiment.Run();
ConsoleHelper.PrintNoWayResults(noWayResult);


// GeneratedPositions generatedPositions;
// Field field;
// CubeState start;
// (int X, int Y) target;
// SearchResult resultBfs;
// SearchResult resultBidirectional;
// SearchResult resultAStar1;
// SearchResult resultAStar2;
//
// while (true)
// {
//     field = Field.Create(rows, columns, wallsCount, random);
//     generatedPositions = PositionGenerator.Generate(field, 27, random);
//
//     start = generatedPositions.Start;
//     target = generatedPositions.Finish;
//
//     resultBfs = CubeSolver1.Solve(field, start, target);
//     resultBidirectional = CubeSolver2.SolveBidirectional(field, start, target);
//     resultAStar1 = CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H1);
//     resultAStar2 = CubeSolver3.SolveAStar(field, start, target, CubeSolver3.H2);
//
//     if (resultBfs.Path is null) continue;
//     if (resultBidirectional.Path is null) continue;
//     if (resultAStar1.Path is null) continue;
//     if (resultAStar2.Path is null) continue;
//
//     break;
// }
//
// ConsoleHelper.PrintBoard(field, start, target);
// ConsoleHelper.PrintStatistic("BFC", resultBfs, start);
// ConsoleHelper.PrintStatistic("Bidirectional", resultBidirectional, start);
// ConsoleHelper.PrintStatistic("AStar 1", resultAStar1, start);
// ConsoleHelper.PrintStatistic("AStar 2", resultAStar2, start);