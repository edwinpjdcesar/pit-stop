# PitStop — Backend

ASP.NET Core Web API for managing vehicle maintenance records.

---

## Tech Stack

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** — ORM with SQL Server
- **FluentValidation** — request validation
- **Scalar** — API reference UI (development only)

---

## Project Structure

```
backend/
├── src/
│   ├── Api/        # Controllers, middleware, app entry point
│   ├── Core/       # Business logic, services, validators
│   ├── Data/       # EF Core context, migrations, seeding
│   └── Domain/     # Entities, DTOs, exceptions
└── tests/
    ├── Api.UnitTests/          # Controller unit tests
    ├── Core.IntegrationTests/  # Service integration tests
    └── Core.UnitTests/         # Validator unit tests
```

---

## API Reference

### Makes

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/makes` | Get all vehicle makes |
| `GET` | `/api/makes/{makeId}/models` | Get all models for a make |

### Vehicles

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/vehicles` | Get all vehicles |
| `GET` | `/api/vehicles/{vehicleId}` | Get a vehicle by ID |
| `POST` | `/api/vehicles` | Create a vehicle |
| `PUT` | `/api/vehicles/{vehicleId}` | Update a vehicle |
| `DELETE` | `/api/vehicles/{vehicleId}` | Delete a vehicle and all its maintenance data |

### Parts

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/parts` | Get all parts |
| `GET` | `/api/parts/{partId}` | Get a part by ID |
| `POST` | `/api/parts` | Create a part |
| `PUT` | `/api/parts/{partId}` | Update a part |
| `DELETE` | `/api/parts/{partId}` | Delete a part |

### Maintenance

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/vehicles/{vehicleId}/maintenance` | Get all maintenance records for a vehicle |
| `POST` | `/api/vehicles/{vehicleId}/maintenance` | Add a maintenance record to a vehicle |
| `PUT` | `/api/vehicles/{vehicleId}/maintenance/{maintenanceId}` | Update a maintenance record |
| `DELETE` | `/api/vehicles/{vehicleId}/maintenance/{maintenanceId}` | Delete a maintenance record |

### Maintenance Parts

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts/{partId}` | Link a part to a maintenance record |
| `PUT` | `/api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts/{partId}` | Update quantity and unit price for a linked part |
| `DELETE` | `/api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts/{partId}` | Remove a part from a maintenance record |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or LocalDB

---

## Getting Started

1. Clone the repository and navigate to the backend folder:

   ```bash
   cd backend
   ```

2. Update the connection string in `src/Api/appsettings.Development.json` if needed. The default targets LocalDB:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PitStop;Trusted_Connection=True;"
   }
   ```

3. Run the API:

   ```bash
   dotnet run --project src/Api
   ```

   On startup, the app applies migrations and seeds the database with vehicle makes and models.

4. Open the API reference at `https://localhost:7226/scalar/v1`.

---

## Running Tests

```bash
dotnet test
```