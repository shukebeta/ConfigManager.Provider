using Microsoft.Extensions.Configuration;

namespace ConfigManager.Provider;

/// <summary>
/// Represents a Redis configuration source for the configuration provider.
/// </summary>
public class RedisConfigurationSource : IConfigurationSource
{
    /// <summary>
    /// Gets or sets the Redis connection string.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379";
    
    /// <summary>
    /// Gets or sets the project name used as key prefix in Redis.
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the Redis database number to use.
    /// </summary>
    public int Database { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets whether the configuration source is optional. If true, connection failures won't cause exceptions.
    /// </summary>
    public bool Optional { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the interval between reconnection attempts.
    /// </summary>
    public TimeSpan ReconnectInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Builds the configuration provider.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <returns>A Redis configuration provider.</returns>
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(ProjectName))
        {
            throw new InvalidOperationException("ProjectName must be specified for Redis configuration source");
        }

        return new RedisConfigurationProvider(this);
    }
}