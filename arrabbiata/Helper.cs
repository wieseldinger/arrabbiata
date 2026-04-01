namespace arrabbiata;

public static class Helper
{
    public static List<int> GetHistory(ArrabbiataContext db, Guid userId)
    {
        var test =
            db.Workouts
                .Where(w => w.Archived == false && w.UserId == userId)
                .OrderByDescending(w => w.WorkoutDate).ToList();
        
        var history =
            db.Workouts
                .Where(w => w.Archived == false && w.ActualTime != null && w.UserId == userId)
                .OrderBy(w => w.WorkoutDate)
                .Select(w => w.ActualTime.Value).ToList();
        
        return history;
    }

    private static List<int> GetArchive(ArrabbiataContext db, Guid userId)
    {
        var archive =
            db.Workouts
                .Where(w => w.Archived == true && w.ActualTime > 25 && w.UserId == userId)
                .OrderBy(w => w.WorkoutDate)
                .Select(w => w.ActualTime.Value).ToList();

        return archive;
    }

    public static bool AddWorkout(ArrabbiataContext db, Workout workout)
    {
        var lastWorkout =
            db.Workouts
                .Where(w => w.Archived == false && w.UserId == workout.UserId)
                .OrderByDescending(w => w.WorkoutDate)
                .Select(w => w.WorkoutType).FirstOrDefault();
        
        if (lastWorkout == workout.WorkoutType)
        {
            return false;
        }

        var tempWorkout = new Workout(workout.UserId, workout.WorkoutType, workout.PlannedTime, workout.ActualTime, DateTime.Now, workout.Tag);
        db.Workouts.Add(tempWorkout);
        return true;
    }

    public static Workout? GetLastWorkout(ArrabbiataContext db, Guid userId)
    {
        var lastWorkout =
            db.Workouts
                .Where(w => w.Archived == false && w.UserId == userId)
                .OrderByDescending(w => w.WorkoutDate).FirstOrDefault();

        return lastWorkout;
    }
    
    public static HistoryStats CalculateStats(ArrabbiataContext db, Guid userId)
    {

        var workouts = GetArchive(db, userId);
        
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