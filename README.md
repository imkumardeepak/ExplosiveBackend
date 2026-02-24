# Peso-Based Barcode Printing System API

A comprehensive ASP.NET Core Web API for managing explosive barcode printing, inventory management, and logistics operations.

## 🏗️ Architecture Overview

This is a robust enterprise-grade API built with modern .NET technologies, designed for handling complex barcode generation, inventory tracking, and dispatch management workflows in the explosives industry.

### Tech Stack
- **Framework**: ASP.NET Core 8.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **Logging**: Serilog with file and console sinks
- **Documentation**: Swagger/OpenAPI
- **Caching**: Redis Distributed Cache
- **Real-time**: WebSocket notifications
- **ORM**: Entity Framework Core with Npgsql provider

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (v13+ recommended)
- [Redis](https://redis.io/download) (for caching)
- Visual Studio 2022 / VS Code

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd Project_SourceCode/Backend
```

2. **Restore NuGet packages**
```bash
dotnet restore
```

3. **Configure database connection**
Update `appsettings.json` with your PostgreSQL connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=Explo_Barcode_DB_Data_test;UID=your_username;PWD=your_password;"
  }
}
```

4. **Run database migrations**
```bash
dotnet ef database update
```

5. **Start Redis server**
```bash
redis-server
```

6. **Run the application**
```bash
dotnet run
```

The API will be available at: `https://localhost:5001` or `http://localhost:5000`

## 📚 API Documentation

### Swagger UI
Access the interactive API documentation at:
```
https://localhost:5001/swagger
```

### Authentication
Most endpoints require JWT authentication. Obtain a token via the Login endpoint:

```http
POST /api/Login/Authenticate
Content-Type: application/json

{
  "username": "your_username",
  "password": "your_password"
}
```

Include the token in subsequent requests:
```http
Authorization: Bearer YOUR_JWT_TOKEN
```

## 🗂️ Project Structure

```
Backend/
├── Controllers/           # API controllers (65+ controllers)
├── Entities/             # Database entities/models
├── DBContext/           # Entity Framework context
├── Interface/           # Repository interfaces
├── Repositorys/         # Repository implementations
├── Services/            # Business logic services
├── Extensions/          # Service collection extensions
├── Middleware/          # Custom middleware
├── Configurations/      # Configuration classes
├── Models/              # DTOs and view models
├── Migrations/          # EF Core migrations
└── wwwroot/PRNFile/     # Printer configuration files
```

## 🔧 Key Features

### Core Modules
- **Barcode Generation**: Multi-level barcode creation (L1, L2, L3)
- **Inventory Management**: Real-time stock tracking and magazine allocation
- **Production Planning**: Manufacturing scheduling and batch management
- **Dispatch Operations**: Loading sheet management and truck dispatch
- **Reporting**: Comprehensive reports for RE forms and production metrics
- **User Management**: Role-based access control with JWT authentication
- **Master Data**: Complete configuration for plants, products, customers, transport

### Technical Features
- **Bulk Operations**: High-performance bulk data processing
- **Real-time Notifications**: WebSocket-based live updates
- **PDF Generation**: Report generation using iText7 and EPPlus
- **Excel Integration**: Data import/export capabilities
- **Caching**: Redis-based distributed caching
- **Logging**: Structured logging with Serilog
- **Error Handling**: Global exception handling middleware

## 🛠️ Configuration

### Environment Variables
Create `appsettings.Development.json` for local development:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=Explo_Barcode_DB_Data_test;UID=postgres;PWD=your_password;"
  },
  "JWTSecret": "your_secret_key_here",
  "JWT": {
    "Issuer": "PesoBarcodeAuthServer",
    "Audience": "PesoBarcodeClients"
  }
}
```

### Redis Configuration
Redis runs on `localhost:6379` by default. Update in `ServiceExtensions.cs` if needed.

## 📊 Database Schema

The system manages complex relationships between:
- **Master Data**: Plants, Products, Brands, Customers, Transport
- **Barcode Hierarchy**: L1 → L2 → L3 barcode relationships
- **Inventory**: Magazine stock tracking with real-time updates
- **Production**: Batch management and manufacturing workflows
- **Dispatch**: Loading sheets, truck assignments, and delivery tracking

## 🔐 Security

### Authentication Flow
1. User authenticates via `/api/Login/Authenticate`
2. JWT token is issued with user claims
3. Token must be included in `Authorization: Bearer` header
4. Token validation occurs on each protected request

### Role-Based Access Control
- **Admin**: Full system access
- **Manager**: Operational access with restrictions
- **Operator**: Limited access to specific modules
- **Viewer**: Read-only access

## 🔄 WebSocket Notifications

Real-time updates are provided via WebSocket endpoint:
```
ws://localhost:5000/ws/notification
```

Subscribe to receive live updates for:
- Barcode generation status
- Inventory changes
- Dispatch updates
- System alerts

## 📈 Performance Optimization

### Database Indexing Strategy
The system implements comprehensive indexing for:
- Barcode lookup operations (L1, L2, L3 relationships)
- Date-based queries (manufacturing, dispatch dates)
- Foreign key relationships
- Composite indexes for complex query patterns

### Caching Strategy
- **Redis Cache**: Frequently accessed master data
- **In-memory Cache**: Short-lived operational data
- **Distributed Sessions**: User session management

## 🐛 Troubleshooting

### Common Issues

**Database Connection Failed**
- Verify PostgreSQL is running
- Check connection string in appsettings.json
- Ensure database exists and user has proper permissions

**Redis Connection Failed**
- Start Redis server: `redis-server`
- Verify Redis is listening on port 6379
- Check firewall settings

**JWT Authentication Issues**
- Verify JWT secret in configuration
- Check token expiration settings
- Ensure correct issuer/audience configuration

**Migration Errors**
```bash
# Remove migration
dotnet ef migrations remove

# Add new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## 📞 Support

For issues and questions:
- Check the [Technical Documentation](TECHNICAL_DOCUMENTATION.md)
- Review [Audit Report](TECHNICAL_AUDIT_REPORT.md)
- Contact system administrator

## 📄 License

This project is proprietary software for Peso Explosives Limited.

## 🔄 Version History

- **v1.0.0**: Initial release with core barcode functionality
- **v1.1.0**: Added RE form generation and reporting
- **v1.2.0**: Implemented real-time notifications and caching
- **v1.3.0**: Enhanced security and performance optimizations

---

*Built with ❤️ for the explosives industry*