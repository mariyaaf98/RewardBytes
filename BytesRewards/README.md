# AppWeaver MinimalMonolith Template

A production-ready modular monolith template built with .NET 8, following Clean Architecture and Domain-Driven Design principles. **Now with .NET Aspire integration** for enhanced observability, service orchestration, and cloud-ready deployment.

## 🚀 Quick Start

### Option 1: Run with .NET Aspire (Recommended)

```bash
# Start the complete application stack with observability
cd src/BytesRewards.AppHost
dotnet run

# Access the application
# - API: http://localhost:7000
# - Swagger: http://localhost:7000/swagger
# - Aspire Dashboard: http://localhost:15000
```

### Option 2: Run Service Only (Development)

```bash
# Run just the API with SQLite
cd src/BytesRewards.Service
dotnet run

# Access at: http://localhost:5000
```

## 🏗️ Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│  FastEndpoints • Minimal APIs • Swagger • Middleware       │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│     CQRS • Mediator • Commands • Queries • DTOs            │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│    Entities • Aggregates • Domain Services • Events        │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  EF Core • Repository • Unit of Work • External Services   │
└─────────────────────────────────────────────────────────────┘
```

### .NET Aspire Integration

```
┌─────────────────────────────────────────────────────────────┐
│                    .NET Aspire Host                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   PostgreSQL    │  │      Redis      │  │   Dashboard │ │
│  │   (Database)    │  │    (Cache)      │  │(Observability)│ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                 VerticalSlice API                         │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ Service Defaults│  │   OpenTelemetry │  │   Health    │ │
│  │ (Resilience)    │  │   (Tracing)     │  │   Checks    │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## 🎯 Key Features

### Core Features
- ✅ **Clean Architecture** with clear separation of concerns
- ✅ **Domain-Driven Design** with rich domain models
- ✅ **CQRS + Mediator** pattern for scalable request handling
- ✅ **Repository + Unit of Work** with EF Core
- ✅ **FastEndpoints** for high-performance APIs
- ✅ **Comprehensive Exception Handling** with ProblemDetails
- ✅ **Multi-tenancy Support** with tenant isolation
- ✅ **Audit Logging** with soft delete support

### .NET Aspire Features
- 🔍 **Distributed Tracing** - See request flows across components
- 📊 **Real-time Metrics** - CPU, memory, database performance
- 📝 **Structured Logging** - Correlated logs with trace context
- 🏥 **Health Checks** - Monitor application and dependency health
- 🔄 **Service Discovery** - Automatic connection string management
- 🛡️ **Resilience Patterns** - Retry policies and circuit breakers
- 🐳 **Container Orchestration** - PostgreSQL and Redis containers
- ☁️ **Cloud Deployment** - Azure Container Apps ready

## 📦 Project Structure

```
src/
├── BytesRewards.Service/          # Web API layer
├── BytesRewards.ServiceDefaults/# Aspire defaults
└── BytesRewards.AppHost/      # Aspire orchestration

tests/
└── BytesRewards.Tests/        # Integration tests

```

## 🛠️ Technology Stack

### Core Technologies
- **.NET 10** - Latest LTS version
- **ASP.NET Core** - Web framework
- **Entity Framework Core** - ORM
- **FastEndpoints** - High-performance endpoints
- **MediatR** - CQRS implementation
- **Serilog** - Structured logging

### .NET Aspire Stack
- **OpenTelemetry** - Observability standard
- **PostgreSQL** - Production database
- **Redis** - Distributed caching
- **Docker** - Container orchestration
- **Aspire Dashboard** - Real-time monitoring

### AppWeaver Components
- **AppWeaver.Repository** - Generic repository pattern
- **AppWeaver.Mediator** - Enhanced CQRS
- **AppWeaver.Exceptions** - Centralized error handling
- **AppWeaver.Results** - Result pattern implementation
- **AppWeaver.Tenancy** - Multi-tenant support

## 🚦 Getting Started

### Prerequisites

- .NET 10.0 SDK
- Docker Desktop (for Aspire)
- Visual Studio 2022 17.9+ or VS Code

### Installation

1. **Create new project using AppWeaver CLI**:
   ```bash
   aw netcore project new withsample \
     --name YourCompany.YourProject \
     --output ./YourProject \
     --template VerticalSlice
   ```

2. **Navigate to project**:
   ```bash
   cd YourProject
   ```

3. **Run with Aspire**:
   ```bash
   dotnet run --project src/YourCompany.YourProject.AppHost
   ```

4. **Access applications**:
   - **API**: http://localhost:7000
   - **Swagger**: http://localhost:7000/swagger
   - **Aspire Dashboard**: http://localhost:15000

## 📊 Observability

### Aspire Dashboard Features

1. **Resources View**: Monitor all services and dependencies
2. **Traces View**: Distributed tracing across components
3. **Metrics View**: Real-time performance metrics
4. **Logs View**: Structured logs with correlation
5. **Console Logs**: Container and application logs

### Custom Metrics Example

```csharp
public class TodoService
{
    private static readonly Counter<int> TodosCreated = 
        Meter.CreateCounter<int>("todos.created");

    public async Task<Todo> CreateAsync(CreateTodoRequest request)
    {
        var todo = await _repository.CreateAsync(request);
        TodosCreated.Add(1, new("tenant", request.TenantId));
        return todo;
    }
}
```

## 🔧 Configuration

### Environment Detection

The application automatically detects the runtime environment:

- **Aspire Mode**: Uses PostgreSQL + Redis with full observability
- **Development Mode**: Uses SQLite + in-memory cache

### Configuration Files

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Aspire.json` - Aspire-specific settings

## 🧪 Testing

### Run Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Tests

Tests run against real dependencies when using Aspire:

```csharp
[Test]
public async Task CreateTodo_ShouldPersistToDatabase()
{
    // ✅ Real PostgreSQL database
    // ✅ Real Redis cache
    // ✅ Full observability stack
    var response = await _client.PostAsJsonAsync("/todos", request);
    response.Should().BeSuccessful();
}
```

## 📚 Documentation

- [Aspire Integration Guide](docs/Aspire-Integration-Guide.md) - Detailed usage guide
- [Enterprise Benefits](docs/Aspire-Enterprise-Benefits.md) - Business value analysis
- [AppWeaver Documentation](https://docs.appweaver.dev) - Framework documentation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- [GitHub Issues](https://github.com/appweaver/modular-monolith/issues)
- [Documentation](https://docs.appweaver.dev)
- [Community Forum](https://community.appweaver.dev)
