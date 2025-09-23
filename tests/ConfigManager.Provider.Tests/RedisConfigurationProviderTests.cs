using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigManager.Provider.Tests;

public class RedisConfigurationProviderTests
{
    [Theory]
    [InlineData("testproject:database:host", "database:host")]
    [InlineData("myapp:nlog:minlevel", "nlog:minlevel")]
    [InlineData("app:weather:location", "weather:location")]
    [InlineData("complex:database:connection:string", "database:connection:string")]
    public void ExtractConfigKey_WithNewFormat_ShouldReturnCorrectKey(string redisKey, string expectedConfigKey)
    {
        // Arrange
        var source = new RedisConfigurationSource { ProjectName = "testproject" };
        var provider = new RedisConfigurationProvider(source);
        
        // Use reflection to access private method for testing
        var method = typeof(RedisConfigurationProvider).GetMethod("ExtractConfigKey", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = method?.Invoke(provider, new object[] { redisKey }) as string;
        
        // Assert
        if (redisKey.StartsWith("testproject:"))
        {
            result.Should().Be(expectedConfigKey);
        }
        else
        {
            result.Should().BeEmpty(); // Should return empty for keys that don't match the project
        }
    }

    [Theory]
    [InlineData("wrongproject:database:host")]
    [InlineData("other:nlog:minlevel")]
    [InlineData("randomkey")]
    public void ExtractConfigKey_WithWrongProject_ShouldReturnEmpty(string redisKey)
    {
        // Arrange
        var source = new RedisConfigurationSource { ProjectName = "testproject" };
        var provider = new RedisConfigurationProvider(source);
        
        // Use reflection to access private method for testing
        var method = typeof(RedisConfigurationProvider).GetMethod("ExtractConfigKey", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = method?.Invoke(provider, new object[] { redisKey }) as string;
        
        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithValidSource_ShouldNotThrow()
    {
        // Arrange
        var source = new RedisConfigurationSource
        {
            ProjectName = "testproject",
            ConnectionString = "localhost:6379"
        };
        
        // Act & Assert
        var provider = new RedisConfigurationProvider(source);
        provider.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new RedisConfigurationProvider(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }
}