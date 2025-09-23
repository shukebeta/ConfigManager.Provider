using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace ConfigManager.Provider;

/// <summary>
/// Configuration provider that loads configuration values from Redis and supports real-time updates via pub/sub.
/// </summary>
public class RedisConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly RedisConfigurationSource _source;
    private IDatabase? _database;
    private ISubscriber? _subscriber;
    private ConnectionMultiplexer? _redis;
    private readonly SemaphoreSlim _reloadLock = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the RedisConfigurationProvider.
    /// </summary>
    /// <param name="source">The configuration source containing Redis connection details.</param>
    public RedisConfigurationProvider(RedisConfigurationSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Loads configuration data from Redis and sets up real-time update subscriptions.
    /// </summary>
    public override void Load()
    {
        try
        {
            ConnectToRedis();
            LoadConfiguration();
            SubscribeToChanges();
        }
        catch (Exception ex)
        {
            if (_source.Optional)
            {
                Data.Clear();
                return;
            }
            throw new InvalidOperationException($"Failed to load Redis configuration: {ex.Message}", ex);
        }
    }

    private void ConnectToRedis()
    {
        var options = ConfigurationOptions.Parse(_source.ConnectionString);
        options.AbortOnConnectFail = false;
        
        _redis = ConnectionMultiplexer.Connect(options);
        _database = _redis.GetDatabase(_source.Database);
        _subscriber = _redis.GetSubscriber();
    }

    private void LoadConfiguration()
    {
        if (_database == null) return;

        var pattern = $"{_source.ProjectName}:*";
        var server = _redis!.GetServer(_redis.GetEndPoints().First());
        var keys = server.Keys(pattern: pattern);

        var newData = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var key in keys)
        {
            var value = _database.StringGet(key);
            if (value.HasValue)
            {
                var configKey = ExtractConfigKey(key!);
                if (!string.IsNullOrEmpty(configKey))
                {
                    newData[configKey] = value!;
                }
            }
        }

        Data = newData;
    }

    private string ExtractConfigKey(string redisKey)
    {
        // Convert "projectname:group:setting" to "group:setting"
        var prefix = $"{_source.ProjectName}:";
        if (redisKey.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return redisKey[prefix.Length..];
        }
        return string.Empty;
    }

    private void SubscribeToChanges()
    {
        if (_subscriber == null) return;

        var pattern = $"{_source.ProjectName}:*";
        
        _subscriber.Subscribe(RedisChannel.Pattern(pattern), 
            async (channel, value) =>
            {
                await _reloadLock.WaitAsync();
                try
                {
                    LoadConfiguration();
                    OnReload();
                }
                finally
                {
                    _reloadLock.Release();
                }
            });
    }

    /// <summary>
    /// Releases all resources used by the RedisConfigurationProvider.
    /// </summary>
    public void Dispose()
    {
        _reloadLock?.Dispose();
        _subscriber?.Unsubscribe(RedisChannel.Pattern($"{_source.ProjectName}:*"));
        _redis?.Dispose();
    }
}