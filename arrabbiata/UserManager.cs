/*using Microsoft.EntityFrameworkCore;

namespace arrabbiata;

public class UserManager(ArrabbiataContext db)
{
    private static List<User> _users = new();

    public static bool CheckUser(Guid userId)
    {
        var result = _users.FirstOrDefault(x => x.Id == userId);
        return result is not null;
    }

    public static void CreateUser(Guid userId)
    {
        if (CheckUser(userId)) throw new Exception("User id already exists");
        _users.Add(new User(userId));
    }

    public static void AddWorkout(Guid userId, Workout workout)
    {
        var user = GetUser(userId);
        
        user.AddWorkout(workout);
    }

    public static List<int> GetHistory(Guid userId)
    {
        var user = GetUser(userId);
        
        var history =
            db.Workouts
                .Where(w => w.Archived == false && w.ActualTime != null)
                .OrderBy(w => w.WorkoutDate)
                .Select(w => w.ActualTime.Value).ToList();

        return history;
    }

    public static void ArchiveWorkouts(Guid userId)
    {
        var user = GetUser(userId);
        
        user.ArchiveHistory();
    }

    private static User GetUser(Guid userId)
    {
        return _users.FirstOrDefault(x => x.Id == userId) ?? throw new Exception("User is not existing");
    }
    
}*/