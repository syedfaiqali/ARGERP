# Organization.Service - Phase 2.1

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-proprietary-red)]()
[![Status](https://img.shields.io/badge/status-active-green)]()

A comprehensive microservice for managing organizational hierarchy, campuses, departments, designations, and facility management for the EDXS platform.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Development](#development)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

Organization.Service is a **CQRS-based microservice** built with:
- **Framework:** ASP.NET Core 9
- **Database:** SQL Server with Entity Framework Core 9
- **Pattern:** CQRS (Command Query Responsibility Segregation)
- **Dispatcher:** MediatR 12.2
- **Authentication:** JWT Bearer Tokens

### Key Capabilities

| Capability | Details |
|------------|---------|
| **Geographic Hierarchy** | Countries → States → Cities → Districts |
| **Organizational Structure** | Entity → Campuses → Buildings → Floors → Rooms |
| **Org Units** | Departments → Designations (with reporting hierarchy) |
| **Financial** | Cost Centers with budget tracking |
| **Business** | Business Units, Regions |
| **Reference Data** | Languages, Castes, Occupations |

## ✨ Features

### Core Features
- ✅ **Hierarchical Data Management** - Supports multi-level organizational structures
- ✅ **Multi-tenancy** - Client-scoped entities with automatic isolation
- ✅ **Soft Deletes** - Logical deletion with data preservation
- ✅ **Audit Trails** - Track creation, updates, and deletions
- ✅ **Pagination & Search** - Efficient data retrieval with full-text search
- ✅ **JWT Authentication** - Secure token-based access control
- ✅ **Role-Based Authorization** - Admin and tenant-level permissions
- ✅ **RESTful API** - Standard HTTP methods (GET, POST, PUT, DELETE)
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Error Handling** - Comprehensive error responses with status codes

### Data Integrity
- ✅ **Cascade Prevention** - Cannot delete parent records with children
- ✅ **Code Uniqueness** - Duplicate prevention on code fields
- ✅ **Referential Integrity** - Automatic foreign key validation
- ✅ **Constraints** - Business rule enforcement at database level

## 🚀 Quick Start

### Prerequisites
```
.NET 9 SDK
SQL Server 2019+ or LocalDB
Visual Studio 2022 / VS Code + C# Extension
```

### Installation (2 minutes)

1. **Clone & Navigate**
   ```bash
   cd NewVersion/src/Services/Organization.Service
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Create Database**
   ```bash
   # Using EF Core Migrations
   dotnet ef database update
   
   # OR using SQL Script
   # Execute: DatabaseScripts/Phase-2.1-Organization-Migration.sql
   ```

4. **Configure JWT**
   Edit `appsettings.json` - Copy JWT settings from Auth.Service

5. **Run Service**
   ```bash
   dotnet run
   ```

6. **Access Swagger UI**
   Navigate to: `https://localhost:5001/swagger`

## 🏗️ Architecture

### CQRS Pattern

```
Request
  ↓
Controller
  ├── Command (Write) → Handler → Database
  └── Query (Read)   → Handler → Database
  ↓
Response (DTO)
```

### Layer Structure

```
Controllers
    ↓
MediatR (Dispatcher)
    ├── Commands + Handlers
    └── Queries + Handlers
    ↓
DbContext
    ↓
SQL Server
```

### Entity Relationships

**Geographic Hierarchy:**
```
Country (1:N) States (1:N) Cities (1:N) Districts
```

**Organizational Hierarchy:**
```
OrgEntity (1:N) OrgCampus (1:N) OrgBuilding (1:N) OrgFloor (1:N) OrgRoom
```

**Departmental Structure:**
```
OrgDepartment (1:N) OrgDesignation (Self-referencing for hierarchy)
```

## 📡 API Documentation

### Base URL
```
https://localhost:5001/api
```

### Authentication
```
Header: Authorization: Bearer <JWT_TOKEN>
```

### Public Endpoints (No Token Required)

#### Get Cities by Country
```http
GET /City/GetByCountryId/{countryId}?pageSize=50
```

#### Health Check
```http
GET /health
```

### Protected Endpoints (JWT Required)

#### Countries
```http
GET    /Country/GetAll?pageNumber=1&pageSize=10&searchTerm=&includeInactive=false
GET    /Country/{id}
POST   /Country/Create          [PlatformAdmin]
PUT    /Country/{id}            [PlatformAdmin]
DELETE /Country/{id}            [PlatformAdmin]
```

#### Cities
```http
GET /City/{id}
```

#### Campuses
```http
GET /OrgCampus/GetAll?clientId=1&pageNumber=1&pageSize=10
GET /OrgCampus/{id}
```

#### Departments
```http
GET /OrgDepartment/GetAll?clientId=1&pageNumber=1&pageSize=10
```

### Request/Response Examples

**Create Country**
```bash
POST /api/Country/Create
Authorization: Bearer <TOKEN>
Content-Type: application/json

{
  "countryName": "New Zealand",
  "countryCode": "NZ",
  "countryCode3": "NZL",
  "phoneCode": "64",
  "currencyCode": "NZD",
  "currencyName": "New Zealand Dollar",
  "currencySymbol": "$"
}
```

**Response (201 Created)**
```json
{
  "countryId": 6,
  "countryName": "New Zealand",
  "countryCode": "NZ",
  "countryCode3": "NZL",
  "phoneCode": "64",
  "currencyCode": "NZD",
  "currencyName": "New Zealand Dollar",
  "currencySymbol": "$",
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z",
  "createdBy": 1
}
```

## 🗄️ Database Schema

### Tables (17 Total)

| Table | Purpose | Key Field |
|-------|---------|-----------|
| Countries | Geographic reference | CountryId (PK) |
| States | State/Province reference | StateId (PK) |
| Cities | City reference | CityId (PK) |
| Districts | District reference | DistrictId (PK) |
| OrgEntities | Organizations/Institutions | OrgEntityId (PK) |
| OrgCampuses | School/Branch locations | CampusId (PK) |
| OrgBuildings | Building structures | BuildingId (PK) |
| OrgFloors | Floor numbers | FloorId (PK) |
| OrgRooms | Individual rooms | RoomId (PK) |
| OrgDepartments | Departments/Units | DepartmentId (PK) |
| OrgDesignations | Job titles/Roles | DesignationId (PK) |
| OrgCostCenters | Financial tracking | CostCenterId (PK) |
| OrgBusinessUnits | Business divisions | BusinessUnitId (PK) |
| OrgRegions | Geographic regions | RegionId (PK) |
| OrgLanguages | Language reference | LanguageId (PK) |
| OrgCastes | Caste reference | CasteId (PK) |
| Occupations | Occupation reference | OccupationId (PK) |

### Unique Constraints

```sql
UNIQUE(CountryCode)           -- Global
UNIQUE(CountryCode3)          -- Global
UNIQUE(ClientId, StateCode)   -- Per Client
UNIQUE(ClientId, CityCode)    -- Per Client
UNIQUE(ClientId, CampusCode)  -- Per Client
UNIQUE(LanguageCode)          -- Global
UNIQUE(LanguageName)          -- Global
```

## 👨‍💻 Development

### Project Structure

```
Organization.Service/
├── Controllers/              # API endpoints
├── Entities/                 # Domain models
├── DTOs/                     # Data transfer objects
├── Commands/                 # Write operations
├── Queries/                  # Read operations
├── Handlers/                 # Command & Query handlers
│   ├── Commands/
│   └── Queries/
├── Infrastructure/           # EF Core DbContext
├── Services/                 # Domain services
├── Program.cs               # Startup configuration
├── appsettings.json         # Configuration
└── Organization.Service.csproj  # Dependencies
```

### Adding a New Entity

1. **Define Entity** in `Entities/OrganizationEntities.cs`
   ```csharp
   public class MyEntity
   {
       public int MyEntityId { get; set; }
       public string Code { get; set; }
       public string Name { get; set; }
       public DateTime CreatedAt { get; set; }
       // ... other properties
   }
   ```

2. **Add DbSet** in `Infrastructure/OrganizationDbContext.cs`
   ```csharp
   public DbSet<MyEntity> MyEntities { get; set; }
   ```

3. **Configure in DbContext**
   ```csharp
   modelBuilder.Entity<MyEntity>(entity =>
   {
       entity.HasKey(e => e.MyEntityId);
       entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
       // ... configure relationships
   });
   ```

4. **Create DTOs** in `DTOs/OrganizationDTOs.cs`

5. **Create Commands** in `Commands/OrganizationCommands.cs`

6. **Create Queries** in `Queries/OrganizationQueries.cs`

7. **Create Handlers** in `Handlers/Commands/` and `Handlers/Queries/`

8. **Create Controller** in `Controllers/OrganizationControllers.cs`

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter ClassName

# Run with verbose output
dotnet test --verbosity detailed
```

### Building & Publishing

```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release

# Publish
dotnet publish -c Release -o ./publish
```

## ⚠️ Troubleshooting

### Database Connection Failed
```
InvalidOperationException: Unable to connect to SQL Server
```
**Solution:**
```bash
# Verify SQL Server is running
sqlcmd -S (localdb)\mssqllocaldb

# Recreate database
dotnet ef database drop --force
dotnet ef database update
```

### JWT Validation Error
```
InvalidOperationException: Bearer token validation failed
```
**Solution:**
1. Verify Auth.Service is running on correct port
2. Check JWT secret matches in appsettings.json
3. Ensure token from /api/Auth/Login endpoint

### Port Already in Use
```
The process cannot access the file because another process has locked a portion of the file.
```
**Solution:**
```bash
# Kill process on port 5001
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Or use different port
dotnet run --urls="https://localhost:5002"
```

### Migration Failed
```
Failed to update database
```
**Solution:**
```bash
# Check pending migrations
dotnet ef migrations list

# Remove last migration
dotnet ef migrations remove

# Create fresh migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 📚 Documentation

- [Implementation Summary](docs/Phase-2.1-Implementation-Summary.md)
- [Quick Start Guide](docs/Phase-2.1-Quick-Start-Guide.md)
- [Swagger UI](https://localhost:5001/swagger) (when running)

## 📈 Performance Considerations

### Query Optimization
- ✅ Indexes on frequently accessed columns
- ✅ Use `AsNoTracking()` for read-only queries
- ✅ Pagination applied at database level
- ✅ Include/ThenInclude for related entities

### Caching Strategy
- ✅ Reference data cached (Languages, Castes, Occupations)
- ✅ Geographic hierarchy cached
- ✅ TTL: 1 hour for invalidation

### Concurrency
- ✅ Database-level constraints
- ✅ Optimistic concurrency with timestamps
- ✅ Soft delete prevents data loss

## 🔐 Security

### Authentication
- ✅ JWT Bearer tokens from Auth.Service
- ✅ Token validation on every request
- ✅ 60-minute token expiration

### Authorization
- ✅ Role-based access control (RBAC)
- ✅ Admin-only endpoints for mutations
- ✅ Client-scoped data isolation

### Data Protection
- ✅ Soft deletes preserve audit trail
- ✅ CreatedBy/UpdatedBy tracking
- ✅ SQL injection prevention via EF Core
- ✅ HTTPS/TLS for data in transit

## 📝 License

Proprietary - All Rights Reserved © 2024 EDXS

## 🤝 Support

For issues or questions:
1. Check [Troubleshooting](#troubleshooting) section
2. Review [Implementation Summary](docs/Phase-2.1-Implementation-Summary.md)
3. Check API logs in `bin/` or output window

---

**Version:** 1.0 | **Status:** ✅ Production Ready | **Last Updated:** 2024
