# PitStop Design Document

## 1. Overview

PitStop is a vehicle maintenance tracking application. It stores vehicles, the maintenance performed on those vehicles, and the parts used during each maintenance event.

The application supports:

- Read-only reference data for vehicle makes and models
- CRUD operations for vehicles
- CRUD operations for parts
- Vehicle-scoped maintenance records
- Linking parts to maintenance records with quantity and unit price

Authentication is not part of the current scope. The design notes where authentication can be added later without changing the core domain model.

## 2. Domain Model

### 2.1 Make

**Purpose:** Reference data for vehicle manufacturers.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| Id | int | Yes | Primary key, auto-increment |
| Code | string | Yes | System-seeded reference value |
| Name | string | Yes | Display name |

**Relationships**

- One `Make` can have many `Model` records.
- `Make` is referenced by `Vehicle`.

**Lifecycle**

- Read-only reference data
- Seeded by the system
- No user-facing create, update, or delete operations

---

### 2.2 Model

**Purpose:** Reference data for vehicle models.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| Id | int | Yes | Primary key, auto-increment |
| Code | string | Yes | System-seeded reference value |
| Name | string | Yes | Display name |
| MakeId | int | Yes | Foreign key to `Make.Id` |

**Relationships**

- Each `Model` belongs to one `Make`.
- One `Make` can have many `Model` records.
- `Model` is referenced by `Vehicle`.

**Lifecycle**

- Read-only reference data
- Seeded by the system
- No user-facing create, update, or delete operations

---

### 2.3 Vehicle

**Purpose:** A user-managed vehicle record that can have zero or more maintenance records.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| Id | guid | Yes | Primary key |
| VIN | string | No | Optional |
| LicensePlate | string | No | Optional |
| Year | int | Yes | Required |
| MakeId | int | Yes | Foreign key to `Make.Id` |
| ModelId | int | Yes | Foreign key to `Model.Id` |
| PurchaseDate | date | No | Optional |
| PurchasePrice | decimal | No | Optional |
| MileageAtPurchase | int | No | Optional |
| Mileage | int | No | Optional |

> Note: The draft spec lists `Make` and `Model` as string fields that reference the `Make` and `Model` entities. For implementation, store them as `MakeId` and `ModelId` foreign keys and return both IDs and display values in API responses.

**Relationships**

- Each `Vehicle` belongs to one `Make`.
- Each `Vehicle` belongs to one `Model`.
- One `Vehicle` can have zero or many `Maintenance` records.

**Cascade rules**

- Deleting a `Vehicle` deletes all related `Maintenance` records.
- Deleting those `Maintenance` records also deletes related `MaintenancePart` records.

---

### 2.4 Maintenance

**Purpose:** A maintenance event performed on a vehicle.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| Id | guid | Yes | Primary key |
| VehicleId | guid | Yes | Foreign key to `Vehicle.Id` |
| Description | string | Yes | Required |
| Mileage | int | Yes | Required |
| ServiceDate | datetime | Yes | Required |

**Relationships**

- Each `Maintenance` belongs to one `Vehicle`.
- One `Maintenance` can have zero or many `MaintenancePart` records.
- Through `MaintenancePart`, one `Maintenance` can reference many `Part` records.

**Cascade rules**

- `Maintenance` cannot exist without a `Vehicle`.
- Deleting a `Maintenance` record deletes all related `MaintenancePart` records.

---

### 2.5 Part

**Purpose:** A part that may be reused across many maintenance records.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| Id | guid | Yes | Primary key |
| Name | string | Yes | Required |
| ModelNumber | string | No | Optional |
| Description | string | Yes | Required |

**Relationships**

- One `Part` can appear in zero or many `MaintenancePart` records.
- Through `MaintenancePart`, one `Part` can be associated with many `Maintenance` records.

**Cascade rules**

- Deleting a `Part` deletes all related `MaintenancePart` records.
- Deleting a `Part` does **not** delete any `Maintenance` records.

---

### 2.6 MaintenancePart

**Purpose:** Junction entity linking `Maintenance` and `Part`, with per-maintenance quantity and unit price.

| Field | Type | Required | Constraints / Notes |
| --- | --- | --- | --- |
| MaintenanceId | guid | Yes | Foreign key to `Maintenance.Id`; part of composite primary key |
| PartId | guid | Yes | Foreign key to `Part.Id`; part of composite primary key |
| Quantity | int | Yes | Required |
| UnitPrice | decimal | Yes | Required |

**Primary key**

- Composite primary key: (`MaintenanceId`, `PartId`)

**Relationships**

- Each `MaintenancePart` belongs to one `Maintenance`.
- Each `MaintenancePart` belongs to one `Part`.
- The combination of `MaintenanceId` and `PartId` must be unique.

**Cascade rules**

- Deleted automatically when the parent `Maintenance` is deleted
- Deleted automatically when the related `Part` is deleted

---

### 2.7 Relationship Summary

| From | To | Relationship | Delete Behavior |
| --- | --- | --- | --- |
| Make | Model | One-to-many | No user-facing delete |
| Make | Vehicle | One-to-many | No user-facing delete |
| Model | Vehicle | One-to-many | No user-facing delete |
| Vehicle | Maintenance | One-to-many | Cascade delete from Vehicle to Maintenance |
| Maintenance | MaintenancePart | One-to-many | Cascade delete from Maintenance to MaintenancePart |
| Part | MaintenancePart | One-to-many | Cascade delete from Part to MaintenancePart |
| Maintenance | Part | Many-to-many through MaintenancePart | Junction rows deleted on either side delete |

## 3. API Design

### 3.1 Conventions

- Base route prefix: `/api`
- Content type for request and response bodies: `application/json`
- All successful responses return JSON
- All error responses return JSON
- `guid` values are represented as strings
- `date` values use ISO 8601 date format: `YYYY-MM-DD`
- `datetime` values use ISO 8601 timestamp format

### 3.2 Common Error Response Shape

```json
{
  "error": {
    "code": "string",
    "message": "string",
    "details": []
  }
}
```

Suggested status codes:

- `400 Bad Request` for invalid input
- `404 Not Found` for missing resources
- `409 Conflict` for invalid state conflicts
- `500 Internal Server Error` for unexpected failures

### 3.3 Makes

#### `GET /api/makes`

**Purpose:** Get the full list of vehicle makes.

**Request**

- No body
- No route parameters

**Response**

- `200 OK`

```json
[
  {
    "id": 1,
    "code": "HONDA",
    "name": "Honda"
  }
]
```

### 3.4 Models

#### `GET /api/makes/{makeId}/models`

**Purpose:** Get models for a specific make.

**Request**

- Route params:
  - `makeId` (int, required)

**Response**

- `200 OK`

```json
[
  {
    "id": 10,
    "makeId": 1,
    "code": "CIVIC",
    "name": "Civic"
  }
]
```

- `404 Not Found` if `makeId` does not exist

### 3.5 Vehicles

#### `GET /api/vehicles`

**Purpose:** Get all vehicles.

**Request**

- No body

**Response**

- `200 OK`

```json
[
  {
    "id": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
    "vin": "1HGCM82633A123456",
    "licensePlate": "ABC123",
    "year": 2020,
    "make": {
      "id": 1,
      "code": "HONDA",
      "name": "Honda"
    },
    "model": {
      "id": 10,
      "makeId": 1,
      "code": "CIVIC",
      "name": "Civic"
    },
    "purchaseDate": "2023-01-15",
    "purchasePrice": 22000.00,
    "mileageAtPurchase": 15000,
    "mileage": 24500
  }
]
```

#### `GET /api/vehicles/{vehicleId}`

**Purpose:** Get a single vehicle by ID.

**Request**

- Route params:
  - `vehicleId` (guid, required)

**Response**

- `200 OK`

```json
{
  "id": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
  "vin": "1HGCM82633A123456",
  "licensePlate": "ABC123",
  "year": 2020,
  "make": {
    "id": 1,
    "code": "HONDA",
    "name": "Honda"
  },
  "model": {
    "id": 10,
    "makeId": 1,
    "code": "CIVIC",
    "name": "Civic"
  },
  "purchaseDate": "2023-01-15",
  "purchasePrice": 22000.00,
  "mileageAtPurchase": 15000,
  "mileage": 24500
}
```

- `404 Not Found` if the vehicle does not exist

#### `POST /api/vehicles`

**Purpose:** Create a vehicle.

**Request body**

```json
{
  "vin": "1HGCM82633A123456",
  "licensePlate": "ABC123",
  "year": 2020,
  "makeId": 1,
  "modelId": 10,
  "purchaseDate": "2023-01-15",
  "purchasePrice": 22000.00,
  "mileageAtPurchase": 15000,
  "mileage": 24500
}
```

**Response**

- `201 Created`
- `Location: /api/vehicles/{vehicleId}`

```json
{
  "id": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
  "vin": "1HGCM82633A123456",
  "licensePlate": "ABC123",
  "year": 2020,
  "make": {
    "id": 1,
    "code": "HONDA",
    "name": "Honda"
  },
  "model": {
    "id": 10,
    "makeId": 1,
    "code": "CIVIC",
    "name": "Civic"
  },
  "purchaseDate": "2023-01-15",
  "purchasePrice": 22000.00,
  "mileageAtPurchase": 15000,
  "mileage": 24500
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if `makeId` or `modelId` does not exist
- `409 Conflict` if `modelId` does not belong to `makeId`

#### `PUT /api/vehicles/{vehicleId}`

**Purpose:** Update a vehicle.

**Request**

- Route params:
  - `vehicleId` (guid, required)

**Request body**

```json
{
  "vin": "1HGCM82633A123456",
  "licensePlate": "XYZ789",
  "year": 2020,
  "makeId": 1,
  "modelId": 10,
  "purchaseDate": "2023-01-15",
  "purchasePrice": 22000.00,
  "mileageAtPurchase": 15000,
  "mileage": 26000
}
```

**Response**

- `200 OK`

```json
{
  "id": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
  "vin": "1HGCM82633A123456",
  "licensePlate": "XYZ789",
  "year": 2020,
  "make": {
    "id": 1,
    "code": "HONDA",
    "name": "Honda"
  },
  "model": {
    "id": 10,
    "makeId": 1,
    "code": "CIVIC",
    "name": "Civic"
  },
  "purchaseDate": "2023-01-15",
  "purchasePrice": 22000.00,
  "mileageAtPurchase": 15000,
  "mileage": 26000
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the vehicle, make, or model does not exist
- `409 Conflict` if `modelId` does not belong to `makeId`

#### `DELETE /api/vehicles/{vehicleId}`

**Purpose:** Delete a vehicle and all of its maintenance data.

**Request**

- Route params:
  - `vehicleId` (guid, required)

**Response**

- `204 No Content`
- `404 Not Found` if the vehicle does not exist

### 3.6 Maintenance

Maintenance is scoped under a vehicle because it cannot exist on its own.

#### `GET /api/vehicles/{vehicleId}/maintenance`

**Purpose:** Get all maintenance records for a vehicle.

**Request**

- Route params:
  - `vehicleId` (guid, required)

**Response**

- `200 OK`

```json
[
  {
    "id": "d7f35391-4060-4e0f-bd24-f9da2e23fd6e",
    "vehicleId": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
    "description": "Oil change",
    "mileage": 24000,
    "serviceDate": "2026-05-01T10:30:00Z",
    "parts": [
      {
        "partId": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
        "name": "Oil Filter",
        "modelNumber": "OF-123",
        "description": "Standard oil filter",
        "quantity": 1,
        "unitPrice": 12.99
      }
    ]
  }
]
```

- `404 Not Found` if the vehicle does not exist

#### `POST /api/vehicles/{vehicleId}/maintenance`

**Purpose:** Add maintenance to a vehicle.

**Request**

- Route params:
  - `vehicleId` (guid, required)

**Request body**

```json
{
  "description": "Oil change",
  "mileage": 24000,
  "serviceDate": "2026-05-01T10:30:00Z"
}
```

**Response**

- `201 Created`
- `Location: /api/vehicles/{vehicleId}/maintenance/{maintenanceId}`

```json
{
  "id": "d7f35391-4060-4e0f-bd24-f9da2e23fd6e",
  "vehicleId": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
  "description": "Oil change",
  "mileage": 24000,
  "serviceDate": "2026-05-01T10:30:00Z",
  "parts": []
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the vehicle does not exist

#### `PUT /api/vehicles/{vehicleId}/maintenance/{maintenanceId}`

**Purpose:** Update an existing maintenance record for a vehicle.

**Request**

- Route params:
  - `vehicleId` (guid, required)
  - `maintenanceId` (guid, required)

**Request body**

```json
{
  "description": "Oil change and tire rotation",
  "mileage": 24500,
  "serviceDate": "2026-05-01T10:30:00Z"
}
```

**Response**

- `200 OK`

```json
{
  "id": "d7f35391-4060-4e0f-bd24-f9da2e23fd6e",
  "vehicleId": "c9c6aab7-03f8-4fb1-a7f2-02acfcaf6d26",
  "description": "Oil change and tire rotation",
  "mileage": 24500,
  "serviceDate": "2026-05-01T10:30:00Z",
  "parts": []
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the vehicle or maintenance record does not exist
- `409 Conflict` if the maintenance record does not belong to the given vehicle

#### `DELETE /api/vehicles/{vehicleId}/maintenance/{maintenanceId}`

**Purpose:** Remove a maintenance record from a vehicle.

**Request**

- Route params:
  - `vehicleId` (guid, required)
  - `maintenanceId` (guid, required)

**Response**

- `204 No Content`
- `404 Not Found` if the vehicle or maintenance record does not exist
- `409 Conflict` if the maintenance record does not belong to the given vehicle

### 3.7 Parts

#### `GET /api/parts`

**Purpose:** Get all parts.

**Request**

- No body

**Response**

- `200 OK`

```json
[
  {
    "id": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
    "name": "Oil Filter",
    "modelNumber": "OF-123",
    "description": "Standard oil filter"
  }
]
```

#### `GET /api/parts/{partId}`

**Purpose:** Get a part by ID.

**Request**

- Route params:
  - `partId` (guid, required)

**Response**

- `200 OK`

```json
{
  "id": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "name": "Oil Filter",
  "modelNumber": "OF-123",
  "description": "Standard oil filter"
}
```

- `404 Not Found` if the part does not exist

#### `POST /api/parts`

**Purpose:** Create a part.

**Request body**

```json
{
  "name": "Oil Filter",
  "modelNumber": "OF-123",
  "description": "Standard oil filter"
}
```

**Response**

- `201 Created`
- `Location: /api/parts/{partId}`

```json
{
  "id": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "name": "Oil Filter",
  "modelNumber": "OF-123",
  "description": "Standard oil filter"
}
```

- `400 Bad Request` for invalid payload

#### `PUT /api/parts/{partId}`

**Purpose:** Update a part.

**Request**

- Route params:
  - `partId` (guid, required)

**Request body**

```json
{
  "name": "Premium Oil Filter",
  "modelNumber": "OF-123",
  "description": "Premium filter for synthetic oil"
}
```

**Response**

- `200 OK`

```json
{
  "id": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "name": "Premium Oil Filter",
  "modelNumber": "OF-123",
  "description": "Premium filter for synthetic oil"
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the part does not exist

#### `DELETE /api/parts/{partId}`

**Purpose:** Delete a part and remove it from all maintenance records.

**Request**

- Route params:
  - `partId` (guid, required)

**Response**

- `204 No Content`
- `404 Not Found` if the part does not exist

### 3.8 Maintenance Parts

MaintenancePart is managed as a nested resource under a maintenance record.

#### `POST /api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts`

**Purpose:** Add an existing part to a maintenance record.

**Request**

- Route params:
  - `vehicleId` (guid, required)
  - `maintenanceId` (guid, required)

**Request body**

```json
{
  "partId": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "quantity": 1,
  "unitPrice": 12.99
}
```

**Response**

- `201 Created`

```json
{
  "maintenanceId": "d7f35391-4060-4e0f-bd24-f9da2e23fd6e",
  "partId": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "quantity": 1,
  "unitPrice": 12.99
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the vehicle, maintenance, or part does not exist
- `409 Conflict` if the maintenance does not belong to the vehicle
- `409 Conflict` if the part is already linked to the maintenance record

#### `PUT /api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts/{partId}`

**Purpose:** Update quantity and unit price for a part on a maintenance record.

**Request**

- Route params:
  - `vehicleId` (guid, required)
  - `maintenanceId` (guid, required)
  - `partId` (guid, required)

**Request body**

```json
{
  "quantity": 2,
  "unitPrice": 11.50
}
```

**Response**

- `200 OK`

```json
{
  "maintenanceId": "d7f35391-4060-4e0f-bd24-f9da2e23fd6e",
  "partId": "dc71a547-85bd-4a48-9fd0-11af6a0cc0b6",
  "quantity": 2,
  "unitPrice": 11.50
}
```

- `400 Bad Request` for invalid payload
- `404 Not Found` if the vehicle, maintenance, part, or link does not exist
- `409 Conflict` if the maintenance does not belong to the vehicle

#### `DELETE /api/vehicles/{vehicleId}/maintenance/{maintenanceId}/parts/{partId}`

**Purpose:** Remove a part from a maintenance record without deleting the part itself.

**Request**

- Route params:
  - `vehicleId` (guid, required)
  - `maintenanceId` (guid, required)
  - `partId` (guid, required)

**Response**

- `204 No Content`
- `404 Not Found` if the vehicle, maintenance, part, or link does not exist
- `409 Conflict` if the maintenance does not belong to the vehicle

## 4. Business Rules

### 4.1 Reference Data Rules

- `Make` and `Model` are read-only.
- `Make` and `Model` data is seeded by the system.
- There is no user-facing CRUD for `Make` or `Model`.
- Models are queried in the context of a specific make.

### 4.2 Vehicle Rules

- `Vehicle.Id` is a GUID.
- `Year` is required.
- `MakeId` is required and must reference an existing `Make`.
- `ModelId` is required and must reference an existing `Model`.
- The selected `Model` must belong to the selected `Make`.
- `VIN` is optional.
- `LicensePlate` is optional.
- `PurchaseDate` is optional.
- `PurchasePrice` is optional.
- `MileageAtPurchase` is optional.
- `Mileage` is optional.
- A vehicle can exist with zero maintenance records.

### 4.3 Maintenance Rules

- `Maintenance.Id` is a GUID.
- `Maintenance` cannot be created without a vehicle.
- `VehicleId` is required.
- `Description` is required.
- `Mileage` is required.
- `ServiceDate` is required.
- A maintenance record belongs to exactly one vehicle.
- A maintenance record can exist with zero linked parts.
- Maintenance endpoints must enforce vehicle ownership of the maintenance record.

### 4.4 Part Rules

- `Part.Id` is a GUID.
- `Name` is required.
- `Description` is required.
- `ModelNumber` is optional.
- A part can exist without being linked to any maintenance record.
- A part may be reused in many maintenance records.

### 4.5 MaintenancePart Rules

- `MaintenancePart` uses a composite primary key: (`MaintenanceId`, `PartId`).
- `MaintenancePart` cannot exist without both a valid `Maintenance` and a valid `Part`.
- `Quantity` is required.
- `UnitPrice` is required.
- The same `Part` cannot be added twice to the same `Maintenance`.
- Removing a part from a maintenance record deletes only the junction row, not the `Part`.
- Maintenance-part endpoints must enforce vehicle ownership of the parent maintenance record.

## 5. Data Integrity Notes

### 5.1 Foreign Keys

- `Model.MakeId` -> `Make.Id`
- `Vehicle.MakeId` -> `Make.Id`
- `Vehicle.ModelId` -> `Model.Id`
- `Maintenance.VehicleId` -> `Vehicle.Id`
- `MaintenancePart.MaintenanceId` -> `Maintenance.Id`
- `MaintenancePart.PartId` -> `Part.Id`

### 5.2 Delete Behavior

| Deleted Entity | Dependent Entity | Behavior |
| --- | --- | --- |
| Vehicle | Maintenance | Cascade delete |
| Maintenance | MaintenancePart | Cascade delete |
| Part | MaintenancePart | Cascade delete |
| MaintenancePart | Part | No effect |
| MaintenancePart | Maintenance | No effect |

### 5.3 Required Database Constraints

- Primary key on each root entity (`Make`, `Model`, `Vehicle`, `Maintenance`, `Part`)
- Composite primary key on `MaintenancePart` (`MaintenanceId`, `PartId`)
- Foreign key constraints for all relationships listed above
- Unique identity of each maintenance-part pair through the composite key
- Non-null constraints on all required fields

### 5.4 Deletion Outcomes

- Deleting a vehicle removes:
  - The vehicle row
  - All maintenance rows for that vehicle
  - All maintenance-part rows attached to those maintenance rows
- Deleting a maintenance record removes:
  - The maintenance row
  - All maintenance-part rows for that maintenance row
- Deleting a part removes:
  - The part row
  - All maintenance-part rows that reference that part
- Deleting a part does not remove:
  - Any maintenance rows
  - Any vehicle rows

## 6. Future Considerations

### 6.1 Authentication and Authorization

Authentication is out of scope for the current version, but the API should leave clear hook-in points for it later.

Recommended future hook-in points:

- Add authentication middleware at the `/api` boundary
- Add authorization checks at the resource/service layer
- Associate vehicles with an owner or account once user identity exists
- Filter vehicle, maintenance, and part access based on the authenticated user
- Add audit fields later if needed, such as:
  - `CreatedBy`
  - `CreatedAt`
  - `UpdatedBy`
  - `UpdatedAt`

### 6.2 Reference Data Management

- `Make` and `Model` are currently seeded system data.
- If admin management is ever needed, add separate admin-only endpoints rather than exposing public CRUD.

### 6.3 Validation Expansion

The draft spec does not define numeric ranges or string length limits. These can be added later without changing the API structure. Examples:

- Minimum allowed year
- Non-negative mileage and price rules
- Maximum lengths for VIN, license plate, name, and description fields

### 6.4 API Evolution

- The current API uses nested routes for maintenance because maintenance cannot exist outside a vehicle.
- The current API uses nested routes for maintenance parts because the junction row only makes sense within a maintenance record.
- If reporting needs grow later, read-only summary endpoints can be added without changing the write model.
