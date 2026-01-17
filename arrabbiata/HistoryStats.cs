namespace arrabbiata;

public struct HistoryStats
{
    public int PauseCount { get; init; }
    public int WorkCount { get; init; }
    public int TotalRuns {  get; init; }

    public HistoryStats(int pauseCount, int workCount, int totalRuns)
        => (PauseCount, WorkCount, TotalRuns) = (pauseCount, workCount, totalRuns);
}