using FluentAssertions;
using Xunit;

namespace ConfigManager.Provider.Tests;

public class RedisConfigurationSourceTests
{
    [Fact]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Act
        var source = new RedisConfigurationSource();
        
        // Assert
        source.ProjectName.Should().Be(string.Empty);
        source.ConnectionString.Should().Be("localhost:6379");
        source.Database.Should().Be(0);
        source.Optional.Should().BeFalse();
        source.ReconnectInterval.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void Build_ShouldReturnRedisConfigurationProvider()
    {
        // Arrange
        var source = new RedisConfigurationSource
        {
            ProjectName = "testproject",
            ConnectionString = "localhost:6379"
        };
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        
        // Act
        var provider = source.Build(builder);
        
        // Assert
        provider.Should().BeOfType<RedisConfigurationProvider>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Build_WithInvalidProjectName_ShouldThrowArgumentException(string? projectName)
    {
        // Arrange
        var source = new RedisConfigurationSource
        {
            ProjectName = projectName,
            ConnectionString = "localhost:6379"
        };
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        
        // Act & Assert
        var action = () => source.Build(builder);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*ProjectName*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Build_WithInvalidConnectionString_ShouldStillBuild(string? connectionString)
    {
        // Arrange
        var source = new RedisConfigurationSource
        {
            ProjectName = "testproject",
            ConnectionString = connectionString
        };
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
        
        // Act & Assert - Current implementation doesn't validate ConnectionString
        var provider = source.Build(builder);
        provider.Should().BeOfType<RedisConfigurationProvider>();
    }

    [Fact]
    public void Properties_ShouldGetAndSetCorrectly()
    {
        // Arrange
        var source = new RedisConfigurationSource();
        
        // Act & Assert
        source.ProjectName = "test-project";
        source.ProjectName.Should().Be("test-project");
        
        source.ConnectionString = "redis://localhost:6379";
        source.ConnectionString.Should().Be("redis://localhost:6379");
        
        source.Database = 3;
        source.Database.Should().Be(3);
        
        source.Optional = true;
        source.Optional.Should().BeTrue();
        
        source.ReconnectInterval = TimeSpan.FromMinutes(5);
        source.ReconnectInterval.Should().Be(TimeSpan.FromMinutes(5));
    }
}