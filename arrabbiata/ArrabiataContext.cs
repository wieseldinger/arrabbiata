using Microsoft.EntityFrameworkCore;

namespace arrabbiata;

public class ArrabbiataContext : DbContext
{ 
    public string DbPath { get; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public ArrabbiataContext()
    {  
#if DEBUG
    DbPath = "arrabbiata.db";
#else
    DbPath = "/app/data/arrabbiata.db";
#endif
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}