# Contributing to ConfigManager.Provider

Thank you for your interest in contributing to ConfigManager.Provider! This document provides guidelines and information for contributors.

## 🚀 Quick Start

1. **Fork** the repository
2. **Clone** your fork: `git clone https://github.com/yourusername/ConfigManager.Provider.git`
3. **Create** a feature branch: `git checkout -b feature/amazing-feature`
4. **Make** your changes
5. **Test** thoroughly: `dotnet test`
6. **Commit** with clear messages
7. **Push** and create a **Pull Request**

## 🏗️ Development Setup

### Prerequisites
- .NET 8.0 SDK
- Redis server (for integration tests)
- Your favorite IDE (VS Code, Visual Studio, Rider)

### Building
```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Create package
dotnet pack src/ConfigManager.Provider
```

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "TestMethodName"
```

### Writing Tests
- Use **xUnit** framework
- Use **FluentAssertions** for readable assertions
- Use **Moq** for mocking when needed
- Follow **AAA pattern** (Arrange, Act, Assert)
- Test both **happy path** and **error scenarios**

## 📝 Code Style

### General Guidelines
- Follow **standard C# conventions**
- Use **meaningful names** for variables and methods
- Add **XML documentation** for public APIs
- Keep methods **small and focused**
- Prefer **composition over inheritance**

### Example
```csharp
/// <summary>
/// Configures Redis connection settings.
/// </summary>
/// <param name="connectionString">Redis connection string</param>
/// <param name="database">Database number (default: 0)</param>
public void ConfigureRedis(string connectionString, int database = 0)
{
    // Implementation
}
```

## 🐛 Bug Reports

Use our **bug report template** when creating issues:
- Clear **description** of the problem
- **Steps to reproduce**
- **Expected vs actual** behavior
- **Environment details** (.NET version, Redis version, OS)
- **Error messages** and stack traces

## ✨ Feature Requests

Use our **feature request template**:
- **Use case** description
- **Proposed API** design (if applicable)
- **Integration considerations**
- **Performance implications**

## 🔄 Pull Request Process

### Before Submitting
- [ ] Tests pass: `dotnet test`
- [ ] Code builds: `dotnet build`
- [ ] No new warnings
- [ ] XML documentation updated
- [ ] CHANGELOG.md updated (for notable changes)

### PR Guidelines
- **Clear title** and description
- **Reference issues** (Fixes #123)
- **Small, focused** changes
- **Test coverage** for new features
- **Documentation** updates when needed

### Review Process
1. **Automated checks** must pass (CI/CD)
2. **Code review** by maintainers
3. **Testing** on different environments
4. **Merge** after approval

## 🏷️ Versioning

We follow **Semantic Versioning** (SemVer):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

## 📦 Release Process

### For Maintainers
1. Update version in `ConfigManager.Provider.csproj`
2. Update `CHANGELOG.md`
3. Create GitHub release with tag (e.g., `v1.2.0`)
4. GitHub Actions automatically publishes to NuGet.org

## 🤝 Community Guidelines

### Be Respectful
- **Welcome newcomers** warmly
- **Provide constructive** feedback
- **Celebrate contributions** of all sizes
- **Help others learn** and grow

### Communication
- Use **GitHub Issues** for bugs and features
- Use **GitHub Discussions** for questions
- Be **patient and helpful** in responses

## 📚 Resources

- **Documentation**: [README.md](README.md)
- **Example App**: [src/ConfigManager.ExampleApp](src/ConfigManager.ExampleApp)
- **End-to-End Guide**: [E2E-TEST.md](src/ConfigManager.ExampleApp/E2E-TEST.md)
- **NuGet Package**: https://www.nuget.org/packages/ConfigManager.Provider/

## 🎯 Areas Needing Help

- **Integration tests** with real Redis scenarios
- **Performance benchmarks** and optimizations  
- **Documentation** improvements and examples
- **Additional configuration providers** (Azure Key Vault, etc.)
- **Community examples** and use cases

## 🙏 Recognition

All contributors will be:
- **Listed** in our contributors section
- **Thanked** in release notes
- **Celebrated** for their valuable contributions

---

**Happy Contributing!** 🎉

Every contribution, no matter how small, makes ConfigManager.Provider better for the entire .NET community.