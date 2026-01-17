namespace arrabbiata;

public class ApiResponse(Workout workout, List<int> workouts, HistoryStats stats)
{
    public Workout? Workout { get; set; } = workout;
    public List<int> Workouts { get; set; } = workouts;
    public HistoryStats Stats { get; set; } = stats;
}