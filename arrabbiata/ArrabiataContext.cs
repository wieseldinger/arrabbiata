using Microsoft.EntityFrameworkCore;

namespace arrabbiata;

public class ArrabbiataContext : DbContext
{ 
    public string DbPath { get; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<User> Users { get; set; }
    

    public ArrabbiataContext()
    {
        DbPath = "arrabbiata.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");
}