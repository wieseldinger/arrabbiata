using arrabbiata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ArrabbiataContext>();

// damit es von der localhost seite funktioniert CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddScoped<WorkoutService>();

var app = builder.Build();

// CORS aktivieren
app.UseCors("AllowAll");

app.MapPost("/api/arrabbiata", (
    [FromBody] Workout workout, 
    WorkoutService service,
    ArrabbiataContext db) =>
    {
        var result = service.ProcessWorkout(workout);
        var history = Helper.GetHistory(db, workout.UserId);
        var stats = Helper.CalculateStats(db, workout.UserId);

        var response = new ApiResponse(result, history, stats);
        
        return Results.Ok(response);
    });

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ArrabbiataContext>();
    context.Database.Migrate();
}

app.Run();