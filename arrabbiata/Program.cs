using arrabbiata;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// CORS Policy hinzufügen
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
    return Results.Ok(result);
});

app.Run();