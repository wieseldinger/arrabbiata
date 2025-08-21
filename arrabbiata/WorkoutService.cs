namespace arrabbiata;

public class WorkoutService
{
    private static readonly int SmallPauseTime = 300;
    private static readonly int BigPauseTime = 1500;
    private static readonly int WorkTime = 1500;
    private static readonly int SmallPauseMultiplier = 10;
    private static readonly int BigPauseMultiplier = 4;
    private static readonly int ThresholdMin = 600;
    private static readonly int ThresholdMax = 1200;
    private static readonly int BigBrakeBufferTime = 3;
    
    public Workout ProcessWorkout(Workout workout)
    {
        Guid userId = workout.UserId;
        
        //If Workup type is null its either:
            //first workout of the day
            //or
            //new user
        if (workout.WorkoutType == null)
        {
            if (!UserManager.CheckUser(userId))
            {
                UserManager.CreateUser(userId);
            }
            else
            {
                //Existing streak gets archived to start new one
                    //(you wouldn't want yesterday's time bonus to influence you today's session
                UserManager.ArchiveWorkouts(userId);
            }
            Workout newWorkout = new Workout(userId, WorkoutType.Work, WorkTime, null, DateTime.Now);
            return newWorkout;
        }

        
        
        //If anything was done, workout gets saved at user id
        UserManager.AddWorkout(userId, workout);

        //if workout was pause -> next will be work -> always 25 mins
        if (workout.WorkoutType == WorkoutType.Pause)
        {
            Workout newWorkout = new Workout(userId, WorkoutType.Work, WorkTime, null, DateTime.Now);
            return newWorkout;
        }

        var history = UserManager.GetHistory(userId);

        //if next pause is Small then:
            //then calculate small pause time + bonus from last work
        //if next pause is Big then:
            //then calculate big pause time as result from the last works done! (at least 25 mins)
        if (!CheckForBigPause(history))
        {
            int extraTime = ((history.LastOrDefault() - WorkTime) / SmallPauseMultiplier);
            int newPauseTime = SmallPauseTime + extraTime;
            
            Workout newWorkout = new Workout(userId, WorkoutType.Pause, newPauseTime, null, DateTime.Now);
            return newWorkout;
        }
        else
        {
            Workout newWorkout = new Workout(userId, WorkoutType.Pause, CalculateBigPause(history), null, DateTime.Now);
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