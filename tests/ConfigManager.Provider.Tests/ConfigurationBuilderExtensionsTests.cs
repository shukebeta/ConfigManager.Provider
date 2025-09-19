using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigManager.Provider.Tests;

public class ConfigurationBuilderExtensionsTests
{
    [Fact]
    public void AddRedis_WithProjectNameAndConnectionString_ShouldAddRedisSource()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act
        var result = builder.AddRedis("testproject", "localhost:6379");
        
        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(builder);
        
        var sources = builder.Sources;
        sources.Should().HaveCount(1);
        sources[0].Should().BeOfType<RedisConfigurationSource>();
        
        var redisSource = (RedisConfigurationSource)sources[0];
        redisSource.ProjectName.Should().Be("testproject");
        redisSource.ConnectionString.Should().Be("localhost:6379");
        redisSource.Optional.Should().BeFalse(); // Default value
    }

    [Fact]
    public void AddRedis_WithOptionalParameter_ShouldSetOptionalCorrectly()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act
        builder.AddRedis("testproject", "localhost:6379", optional: true);
        
        // Assert
        var redisSource = (RedisConfigurationSource)builder.Sources[0];
        redisSource.Optional.Should().BeTrue();
    }

    [Fact]
    public void AddRedis_WithNullProjectName_ShouldAcceptButValidateAtBuildTime()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act - Extension method accepts null, validation happens at Build() time
        builder.AddRedis(null!, "localhost:6379");
        
        // Assert
        var action = () => builder.Build();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ProjectName*");
    }

    [Fact]
    public void AddRedis_WithEmptyProjectName_ShouldAcceptButValidateAtBuildTime()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act - Extension method accepts empty, validation happens at Build() time
        builder.AddRedis("", "localhost:6379");
        
        // Assert
        var action = () => builder.Build();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ProjectName*");
    }

    [Fact]
    public void AddRedis_WithNullConnectionString_ShouldAcceptAndUseDefault()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act
        builder.AddRedis("testproject", null!);
        
        // Assert - Should work since ConnectionString has a default value
        var redisSource = (RedisConfigurationSource)builder.Sources[0];
        redisSource.ConnectionString.Should().BeNull(); // The null was accepted
    }

    [Fact]
    public void AddRedis_WithActionDelegate_ShouldConfigureSource()
    {
        // Arrange
        var builder = new ConfigurationBuilder();
        
        // Act
        builder.AddRedis(source =>
        {
            source.ProjectName = "configured-project";
            source.ConnectionString = "configured-connection";
            source.Database = 5;
            source.Optional = true;
        });
        
        // Assert
        var redisSource = (RedisConfigurationSource)builder.Sources[0];
        redisSource.ProjectName.Should().Be("configured-project");
        redisSource.ConnectionString.Should().Be("configured-connection");
        redisSource.Database.Should().Be(5);
        redisSource.Optional.Should().BeTrue();
    }
}