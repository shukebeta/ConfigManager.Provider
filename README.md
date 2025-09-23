# ConfigManager.Provider

A .NET Configuration Provider that enables dynamic configuration management with real-time updates from Redis.

[![NuGet](https://img.shields.io/nuget/v/ConfigManager.Provider.svg)](https://www.nuget.org/packages/ConfigManager.Provider/)
[![Downloads](https://img.shields.io/nuget/dt/ConfigManager.Provider.svg)](https://www.nuget.org/packages/ConfigManager.Provider/)

## Features

- ✅ **Seamless Integration**: Works with standard .NET IConfiguration
- ✅ **Real-time Updates**: Configuration changes trigger automatic reloads
- ✅ **Project Isolation**: Multi-project support with key prefixing
- ✅ **Performance Optimized**: Efficient Redis operations with pub/sub
- ✅ **Production Ready**: Robust error handling and connection management

## Quick Start

### 1. Install Package

```bash
# Install the latest stable version
dotnet add package ConfigManager.Provider

# Or install specific version
dotnet add package ConfigManager.Provider --version 1.0.0
```

### 2. Basic Usage

```csharp
using ConfigManager.Provider;

var configuration = new ConfigurationBuilder()
    .AddRedis("myapp", "localhost:6379")
    .Build();

// Read configuration
var dbHost = configuration["database:host"];
var logLevel = configuration["logging:level"];
```

### 3. With ASP.NET Core / Generic Host

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddRedis(
            projectName: "myapp",
            connectionString: "localhost:6379",
            optional: true
        );
    })
    .Build();
```

### 4. Real-time Configuration Updates

```csharp
// Monitor configuration changes
ChangeToken.OnChange(configuration.GetReloadToken, () =>
{
    Console.WriteLine("Configuration updated!");
});
```

## Configuration Key Format

Redis keys follow the pattern: `{projectName}:{group}:{setting}`

Examples:
- `myapp:database:host` → `configuration["database:host"]`
- `myapp:logging:level` → `configuration["logging:level"]`
- `myapp:api:timeout` → `configuration["api:timeout"]`

## Advanced Configuration

```csharp
config.AddRedis(source =>
{
    source.ProjectName = "myapp";
    source.ConnectionString = "localhost:6379";
    source.Database = 0;
    source.Optional = false; // Fail if Redis unavailable
    source.ReconnectInterval = TimeSpan.FromSeconds(30);
});
```

## End-to-End Example: Real-Time Weather App

This complete example demonstrates the ConfigManager system working together. Based on our tested ConfigManager.ExampleApp.

### Step 1: ASP.NET Core Application Setup

```csharp
using ConfigManager.Provider;

var builder = WebApplication.CreateBuilder(args);

// Add Redis configuration provider
builder.Configuration.AddRedis(
    projectName: "exampleapp",
    connectionString: "your-redis-server:6379",
    optional: true  // Don't fail if Redis is unavailable
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
});

app.MapGet("/weatherforecast", (IConfiguration config) =>
{
    // Use configuration for real-time weather customization
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
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string Location);
```

### Step 2: Dynamic Configuration via ConfigManager.Web API

```bash
# Set weather location (instantly updates the app)
curl -X POST http://localhost:3001/redis/exampleapp:weather:location \
  -H "Content-Type: application/json" \
  -d '{"value": "San Francisco"}'

# Set temperature range
curl -X POST http://localhost:3001/redis/exampleapp:weather:maxTemp \
  -H "Content-Type: application/json" \
  -d '{"value": "25"}'

curl -X POST http://localhost:3001/redis/exampleapp:weather:minTemp \
  -H "Content-Type: application/json" \
  -d '{"value": "10"}'
```

### Step 3: Verify Real-Time Updates

```bash
# Check current configuration
curl http://localhost:5000/config

# Get weather forecast with new location/temps
curl http://localhost:5000/weatherforecast
```

**Result**: The app immediately reflects the new configuration without restart!

```json
[
  {
    "date": "2024-12-20",
    "temperatureC": 18,
    "summary": "Mild",
    "location": "San Francisco"
  }
]
```

### Step 4: Live Configuration Changes

```bash
# Change location to Tokyo (takes effect in <100ms)
curl -X POST http://localhost:3001/redis/exampleapp:weather:location \
  -H "Content-Type: application/json" \
  -d '{"value": "Tokyo"}'

# Immediately check weather - location updated!
curl http://localhost:5000/weatherforecast
```

## Integration with ConfigManager.Web

This provider works seamlessly with the ConfigManager.Web API:

1. **Set Configuration** via API endpoint
2. **Configuration Auto-Updates** in your .NET app via Redis pub/sub
3. **Project Discovery** via `/projects` endpoint
4. **Automatic Project Registration** when first configuration is set

## Example: Complete Integration

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddRedis("myapp", "redis://your-redis-server:6379")
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        
        // Use configuration in your services
        services.Configure<DatabaseOptions>(configuration.GetSection("database"));
        services.Configure<LoggingOptions>(configuration.GetSection("logging"));
    }
}

public class DatabaseOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string ConnectionString => $"Host={Host};Port={Port}";
}
```

## Error Handling

- **Optional Mode**: Set `optional: true` to gracefully handle Redis unavailability
- **Connection Retry**: Automatic reconnection with configurable intervals
- **Graceful Degradation**: Falls back to default values when Redis is unavailable

## Repository Structure

```
ConfigManager.Provider/
├── src/
│   ├── ConfigManager.Provider/          # Main NuGet package
│   └── ConfigManager.ExampleApp/        # Example ASP.NET Core app
├── tests/                               # Unit and integration tests
├── ConfigManager.Provider.sln          # Solution file
└── README.md                           # This file
```

## Building and Testing

```bash
# Build the entire solution
dotnet build

# Run the example app
dotnet run --project src/ConfigManager.ExampleApp

# Run tests (when available)
dotnet test

# Pack for local testing
dotnet pack src/ConfigManager.Provider
```

## Performance

- **O(1) Project Discovery**: Uses Redis Sets for efficient project lookups
- **Minimal Memory**: Only loads configuration for specified project
- **Pub/Sub Efficiency**: Only subscribes to relevant configuration changes
- **Connection Pooling**: Reuses Redis connections for optimal performance

## License

MIT License - see LICENSE file for details.