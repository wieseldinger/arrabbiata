using System.Text.Json.Serialization;

namespace arrabbiata;

public class Workout
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public WorkoutType? WorkoutType {get; private set; }
    public int? PlannedTime { get; private set; }
    public int? ActualTime { get; private set; }
    public bool Archived { get; private set; } = false;
    public DateTime? WorkoutDate { get; private set; }
    public Tag? Tag { get; private set; }

    [JsonConstructor] // optional, aber klarer
    public Workout(Guid userId, WorkoutType? workoutType, int? plannedTime, int? actualTime, DateTime? workoutDate, Tag? tag)
        => (Id, UserId, WorkoutType, PlannedTime, ActualTime, WorkoutDate, Tag)
            = (Guid.NewGuid(), userId, workoutType, plannedTime, actualTime, workoutDate, tag);
    
    protected Workout() {}

    public void ArchiveWorkout()
    {
        Archived = true;
    }
}

public enum WorkoutType 
{
    Work,
    Pause,
    Continue
}