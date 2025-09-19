using ConfigManager.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add Redis configuration provider
builder.Configuration.AddRedis(
    projectName: "exampleapp",
    connectionString: "seq.shukebeta.eu.org:6379",
    optional: true  // Don't fail if Redis is unavailable
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Configuration demo endpoints
app.MapGet("/config", (IConfiguration config) =>
{
    var allConfig = new Dictionary<string, string?>();
    foreach (var kvp in config.AsEnumerable())
    {
        if (!string.IsNullOrEmpty(kvp.Value))
        {
            allConfig[kvp.Key] = kvp.Value;
        }
    }
    return Results.Json(allConfig);
})
.WithName("GetConfiguration")
.WithOpenApi();

app.MapGet("/config/{key}", (string key, IConfiguration config) =>
{
    var value = config[key];
    return value != null 
        ? Results.Json(new { key, value }) 
        : Results.NotFound(new { error = "Configuration key not found", key });
})
.WithName("GetConfigurationValue")
.WithOpenApi();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (IConfiguration config) =>
{
    // Use configuration to customize the weather forecast
    var location = config["weather:location"] ?? "Unknown";
    var maxTemp = config.GetValue<int>("weather:maxTemp", 55);
    var minTemp = config.GetValue<int>("weather:minTemp", -20);
    
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(minTemp, maxTemp),
            summaries[Random.Shared.Next(summaries.Length)],
            location
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string Location)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
