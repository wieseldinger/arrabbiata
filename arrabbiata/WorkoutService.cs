using Microsoft.EntityFrameworkCore;

namespace arrabbiata;

public class WorkoutService(ArrabbiataContext db)
{
    private const int SmallPauseTime = 300;
    private const int BigPauseTime = 1500;
    private const int WorkTime = 1500;
    private const int SmallPauseMultiplier = 10;
    private const int BigPauseMultiplier = 4;
    private const int ThresholdMin = 600;
    private const int ThresholdMax = 1200;
    private const int BigBrakeBufferTime = 3;

    public Workout ProcessWorkout(Workout workout)
    {
        Guid userId = workout.UserId;
        
        //If Workup type is null its either:
            //first workout of the day
            //or
            //new user
        if (workout.WorkoutType is null)
        {
            if (!db.Users.Any(w => w.Id == userId))
            {
                db.Users.Add(new User(userId));
                db.SaveChanges();
            }
            else
            {
                db.Workouts
                    .Where(w => w.UserId == userId && w.Archived == false)
                    .ExecuteUpdate(setters => setters.SetProperty(w => w.Archived, true));
            }
            Workout newWorkout = new Workout(userId, WorkoutType.Work, WorkTime, null, DateTime.Now, workout.Tag);
            return newWorkout;
        }
        
        //tag handling
        Tag? realTag = null;

        if (workout.Tag is not null)
        {
            //User can create NEW Tags with an empty Id PLUS the name
            if (workout.Tag.Id == Guid.Empty)
            {
                if (workout.Tag.Name is not null)
                {
                    Guid realId = db.Tags
                        .Where(t => t.Name == workout.Tag.Name)
                        .Select(t => (Guid?)t.Id)
                        .FirstOrDefault()
                    ?? Guid.NewGuid();

                    realTag = new Tag(realId, null);
                }
            }
            else if(db.Tags.Contains(workout.Tag))
            {
                realTag = new Tag(workout.Tag.Id, null);
            }
        }

        //If Workup type is null its:
            //a user that wants to continue an old workout
        if (workout.WorkoutType == WorkoutType.Continue)
        {
            var lastWorkout = Helper.GetLastWorkout(db, userId);

            if (lastWorkout is not null)
                workout = lastWorkout;
            else
                throw new Exception("cant continue non existing training");
        }
        else if (!Helper.AddWorkout(db, new Workout(workout.Id, workout.WorkoutType, workout.PlannedTime, workout.ActualTime, workout.WorkoutDate, realTag)))
        {
            throw new Exception("cant add same workout type in a row");
        }
        
        db.SaveChanges();
        //if workout was pause -> next will be work -> always 25 mins
        if (workout.WorkoutType == WorkoutType.Pause)
        {
            Workout newWorkout = new Workout(userId, WorkoutType.Work, WorkTime, null, DateTime.Now, null);
            return newWorkout;
        }
        
        var history = Helper.GetHistory(db, userId);
        
        //if next pause is Small then:
            //then calculate small pause time + bonus from last work
        //if next pause is Big then:
            //then calculate big pause time as result from the last works done! (at least 25 mins)
        if (!CheckForBigPause(history))
        {
            int extraTime = ((history.LastOrDefault() - WorkTime) / SmallPauseMultiplier);
            int newPauseTime = SmallPauseTime + extraTime;
            
            Workout newWorkout = new Workout(userId, WorkoutType.Pause, newPauseTime, null, DateTime.Now, null);
            return newWorkout;
        }
        else
        {
            Workout newWorkout = new Workout(userId, WorkoutType.Pause, CalculateBigPause(history), null, DateTime.Now, null);
            return newWorkout;
        }
        
        
    }

    //true = Last 3 breaks where SMALL breaks -> now it's time for a BIG one
    //false = In the last 3 breaks also BIG or MEDIUM breaks -> time for another SMALL one
    
    //3 small breaks = time for big brake!
    private static bool CheckForBigPause(List<int> history)
    {
        int pauseCounter = 0;

        //Checking if there are even more than 3 brakes in history
        if (history.Count < BigBrakeBufferTime * 2)
            return false;
        
        for (int i = history.Count-2; i >= 0; i-=2)
        {
            pauseCounter += 1;
            int pauseTime = history[i];
            if (pauseTime >= ThresholdMax && pauseCounter <= 3)
                return false;

            if (pauseTime >= ThresholdMin && pauseCounter <= 2)
                return false;
            
            //Checking if 3 breaks already over
            if (pauseCounter >= BigBrakeBufferTime)
                return true;
        }
        return false;
    }
    
    //Calculates the length of the big pause
    //At least 25 minutes!
    private static int CalculateBigPause(List<int> history)
    {
        int workTotal = 0;
        
        //work total is sum of worked time in history
        for (int i = 0; i < history.Count; i+= 2)
        {
            workTotal += history[i];
        }

        int pauseBonus = workTotal / BigPauseMultiplier;

        //pauses bigger than 10 minutes gets subtracted minus the small Pause time
        for (int i = 1; i < history.Count; i+= 2)
        {
            if (history[i] > ThresholdMin)
            {
                pauseBonus -= (history[i] - SmallPauseTime);
            }
        }

        return Math.Clamp(pauseBonus, BigPauseTime, BigPauseTime*4);
    }
}