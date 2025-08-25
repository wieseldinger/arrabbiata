namespace arrabbiata;

public static class Helper
{
    public static HistoryStats CalculateStats(List<int> workouts)
    {
        int pauseCount = 0;
        int workCount = 0;
        int totalRuns = workouts.Count;
        
        for (int i = 0; i < totalRuns; i++)
        {
            if (i % 2 == 0)
                workCount += workouts[i];
            else
                pauseCount += workouts[i];
        }
        
        return new HistoryStats(pauseCount, workCount, totalRuns);
    }
}