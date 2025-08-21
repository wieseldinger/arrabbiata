using System.Text.Json.Serialization;

namespace arrabbiata;

public class Workout
{
    public Guid UserId { get; private set; }
    public WorkoutType? WorkoutType {get; private set; }
    public int? PlannedTime { get; private set; }
    public int? ActualTime { get; private set; }
    public DateTime? WorkoutDate { get; private set; }
    
    [JsonConstructor] // optional, aber klarer
    public Workout(Guid userId, WorkoutType? workoutType, int? plannedTime, int? actualTime, DateTime? workoutDate)
        => (UserId, WorkoutType, PlannedTime, ActualTime, WorkoutDate)
            = (userId, workoutType, plannedTime, actualTime, workoutDate);
    
    public Workout() {}
}


public enum WorkoutType 
{
    Work,
    Pause
}