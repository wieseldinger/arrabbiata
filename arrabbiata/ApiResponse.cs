namespace arrabbiata;

public class ApiResponse(Workout workout, HistoryStats stats)
{
    public Workout? Workout { get; set; } = workout;
    public HistoryStats Stats { get; set; } = stats;
}