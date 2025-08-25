using arrabbiata;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

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
    WorkoutService service) =>
    {
        var result = service.ProcessWorkout(workout);
        var stats = Helper.CalculateStats(UserManager.GetHistory(workout.UserId));

        var response = new ApiResponse(result, stats);
        
        return Results.Ok(response);
    });

app.Run();