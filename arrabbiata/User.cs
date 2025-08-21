namespace arrabbiata;

public class User
{
    public Guid UserId { get; private set; }
    
    private readonly List<Workout> _history;
    private readonly List<Workout> _archive;

    public User(Guid id)
    {
        _history = new List<Workout>();
        _archive = new List<Workout>();

        UserId = id;
    }

    public IReadOnlyCollection<Workout> GetHistory()
    {
        return _history.AsReadOnly();
    }

    public IReadOnlyCollection<Workout> GetArchive()
    {
        return _archive.AsReadOnly();
    }

    public void ArchiveHistory()
    {
        _archive.AddRange(_history);
    }

    public void AddWorkout(Workout workout)
    {
        var lastWorkout = _history.LastOrDefault();
        if (lastWorkout != null && workout.WorkoutType == lastWorkout.WorkoutType)
        {
            throw new Exception("workout type can't be same as last one");
        }
        
        _history.Add(workout);
    }
}