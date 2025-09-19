using Microsoft.Extensions.Configuration;

namespace ConfigManager.Provider;

/// <summary>
/// Extension methods for IConfigurationBuilder to add Redis configuration support.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds Redis configuration provider to the configuration builder
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="projectName">The project name used as key prefix in Redis</param>
    /// <param name="connectionString">Redis connection string (default: localhost:6379)</param>
    /// <param name="optional">Whether the configuration source is optional (default: false)</param>
    /// <returns>The configuration builder</returns>
    public static IConfigurationBuilder AddRedis(
        this IConfigurationBuilder builder,
        string projectName,
        string connectionString = "localhost:6379",
        bool optional = false)
    {
        return builder.AddRedis(source =>
        {
            source.ProjectName = projectName;
            source.ConnectionString = connectionString;
            source.Optional = optional;
        });
    }

    /// <summary>
    /// Adds Redis configuration provider with custom configuration
    /// </summary>
    /// <param name="builder">The configuration builder</param>
    /// <param name="configureSource">Action to configure the Redis source</param>
    /// <returns>The configuration builder</returns>
    public static IConfigurationBuilder AddRedis(
        this IConfigurationBuilder builder,
        Action<RedisConfigurationSource> configureSource)
    {
        var source = new RedisConfigurationSource();
        configureSource(source);
        builder.Add(source);
        return builder;
    }
}