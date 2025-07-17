# Application Setup Guide

## Prerequisites

- .NET 9.0 SDK or later
- PostgreSQL database
- RabbitMQ server
- MongoDB server
- JetBrains Rider 2025.1.4 (or Visual Studio)
- Docker (optional, for containerized services)

## Opening the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/DiegoModesto/teste-opea.git
   cd teste-opea
    ```

## Open in JetBrains Rider:
- Launch Rider or Visual Studio
- Click "Open" and select the project folder
- Wait for Rider to restore NuGet packages

## Configuration
1. Database Setup (PostgreSQL)

Update appsettings.json with your PostgreSQL connection string:

```json
{
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=YourDbName;Username=postgres;Password=yourpassword"
    }
}
````

2. RabbitMQ Configuration

Ensure RabbitMQ is running locally or update the connection in Program.cs:

```csharp
var factory = new ConnectionFactory { 
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};
```

3. Run Database Migrations

In terminal or Package Manager Console:

```bash
dotnet ef database update
```

## Running the Application
1. In Rider, set the startup project
2. Press `F5` or click the Run button
3. The Worker service will start processing outbox events from PostgreSQL and publishing to RabbitMQ

## Project Structure

- `Worker.cs` - Background service for processing outbox pattern
- `ApplicationDbContext` - Entity Framework context for PostgreSQL
- `OutboxEvents` - Entity for outbox pattern implementation

## Dependencies

- Entity Framework Core (PostgreSQL)
- RabbitMQ.Client
- Microsoft.Extensions.Hosting

## Required NuGet Packages

```bash
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package RabbitMQ.Client
    dotnet add package Microsoft.Extensions.Hosting
```

## Troubleshooting

- If `builder.Build()` returns null, ensure all dependencies are properly registered in DI
- Use fully qualified names for RabbitMQ interfaces to avoid conflicts: `RabbitMQ.Client.IModel`
- Verify PostgreSQL and RabbitMQ services are running before starting the application