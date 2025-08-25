using arrabbiata;

namespace ServiceTester
{
    public class ServiceTests
    {
        private const int SmallPauseTime = 300;
        private const int WorkTime = 1500;
        private const int MediumPauseMin = 600;
        private const int BigPauseMin = 1200;
        private const int BigPauseClampMin = 1500;
        private const int BigPauseClampMax = 6000;

        private readonly WorkoutService _workoutService = new();

        private Workout StartSession(Guid userId)
            => _workoutService.ProcessWorkout(new Workout(userId, null, null, null, null));

        private Workout FinishWork(Guid userId, int actualSeconds)
            => _workoutService.ProcessWorkout(new Workout(userId, WorkoutType.Work, null, actualSeconds, null));

        private Workout FinishPause(Guid userId, int actualSeconds)
            => _workoutService.ProcessWorkout(new Workout(userId, WorkoutType.Pause, null, actualSeconds, null));

        [Fact]
        public void StartNewUser_ReturnsWorkWith25Minutes()
        {
            var uid = Guid.NewGuid();
            var workout = StartSession(uid);
            Assert.Equal(WorkoutType.Work, workout.WorkoutType);
            Assert.Equal(WorkTime, workout.PlannedTime);
        }

        [Fact]
        public void SameWorkoutType_ThrowsException()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);
            FinishWork(uid, WorkTime);

            Assert.Throws<Exception>(() => FinishWork(uid, WorkTime));
        }

        [Fact]
        public void AfterAnyPause_NextIsAlwaysWork25()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);
            FinishWork(uid, WorkTime);
            var work = FinishPause(uid, 99999999);

            Assert.Equal(WorkoutType.Work, work.WorkoutType);
            Assert.Equal(WorkTime, work.PlannedTime);
        }

        [Fact]
        public void SmallPauseAfterStandardWork_IsExactly5()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);
            var pause = FinishWork(uid, WorkTime);

            Assert.Equal(WorkoutType.Pause, pause.WorkoutType);
            Assert.Equal(SmallPauseTime, pause.PlannedTime);
        }

        [Fact]
        public void BigPauseNotTriggered_BeforeThreeCompletedSmallBreaks()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);

            //Work and Pause 1
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work and Pause 2
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work and Pause 3
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work 4 and Assert return Pause length
            var p4 = FinishWork(uid, WorkTime);
            Assert.True(p4.PlannedTime >= BigPauseClampMin);
        }

        [Fact]
        public void MediumPauseInLastTwo_BlocksBigPause()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);

            //Work and Pause 1 
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work and Pause 2 -> MEDIUM PAUSE 
            FinishWork(uid, WorkTime);
            FinishPause(uid, MediumPauseMin);

            //Work and Pause 3
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work 4 and Assert return Pause length not a BIG PAUSE
            var pauseFour = FinishWork(uid, WorkTime);
            Assert.True(pauseFour.PlannedTime < BigPauseClampMin);
        }

        [Fact]
        public void MediumPauseTwoPauseAgo_DoesntBlocksBigPause()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);

            //Work and Pause 1 -> MEDIUM PAUSE 
            FinishWork(uid, WorkTime);
            FinishPause(uid, MediumPauseMin);

            //Work and Pause 2 
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work and Pause 3
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work 4 and Assert return Pause length IS BIG
            var pauseFour = FinishWork(uid, WorkTime);
            Assert.True(pauseFour.PlannedTime >= BigPauseClampMin);
        }

        [Fact]
        public void BigPauseInLastThree_BlocksBigPause()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);

            //Work and Pause 1 -> BIG PAUSE 
            FinishWork(uid, WorkTime);
            FinishPause(uid, BigPauseMin);

            //Work and Pause 2 
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work and Pause 3
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);

            //Work 4 and Assert return Pause length IS NOT BIG
            var pauseFour = FinishWork(uid, WorkTime);
            Assert.True(pauseFour.PlannedTime < BigPauseClampMin);
        }

        [Fact]
        public void BigPause_RespectsClamp()
        {
            var uid = Guid.NewGuid();
            StartSession(uid);

            // Drei SMALL vorbereiten
            FinishWork(uid, WorkTime);
            FinishPause(uid, SmallPauseTime);
            FinishWork(uid, WorkTime + 3600);
            FinishPause(uid, SmallPauseTime);
            FinishWork(uid, WorkTime + 3600);
            FinishPause(uid, SmallPauseTime);

            var big = FinishWork(uid, 999999999);

            Assert.True(big.PlannedTime >= BigPauseClampMin, "Min 25 min");
            Assert.True(big.PlannedTime <= BigPauseClampMax, "Max 100 min");
        }
    }
}
  