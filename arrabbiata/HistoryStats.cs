namespace arrabbiata;

public struct HistoryStats
{
    public static int PauseCount { get; private set; }
    public static int WorkCount { get; private set; }
    public static int TotalRuns { get; private set; }

    public HistoryStats(int pauseCount, int workCount, int totalRuns)
        => (PauseCount, WorkCount, TotalRuns) = (pauseCount, workCount, totalRuns);
}