# PitStop Frontend Storyboard

## Navigation / Flow Summary

- `/` Vehicle List (landing page) → open a vehicle, create a vehicle, or delete a vehicle.
- `/vehicles/new` → create a vehicle, then go to that vehicle's detail page.
- `/vehicles/:vehicleId` → review one vehicle, edit it, delete it, or manage its maintenance history.
- `/vehicles/:vehicleId/edit` → update vehicle details, then return to the vehicle detail page.
- `/vehicles/:vehicleId/maintenance/new` → create a maintenance record for the selected vehicle.
- `/vehicles/:vehicleId/maintenance/:maintenanceId` → review one maintenance record, edit it, delete it, and manage linked parts.
- `/vehicles/:vehicleId/maintenance/:maintenanceId/edit` → update the maintenance record, then return to its detail page.
- `/vehicles/:vehicleId/maintenance/:maintenanceId/parts/add` → link an existing part to the maintenance record with quantity and unit price.
- `/vehicles/:vehicleId/maintenance/:maintenanceId/parts/:partId/edit` → update quantity and unit price for a linked part.
- `/parts` → browse the reusable parts library, create parts, edit parts, or delete parts.
- `/parts/new` → create a reusable part, then return to the parts library.
- `/parts/:partId/edit` → update a reusable part, then return to the parts library.

Reference data note: Makes and Models do not have dedicated screens. They are selected inside vehicle forms. The Make list comes from `GET /api/makes`, and the Model list refreshes from `GET /api/makes/{makeId}/models` after a Make is chosen.

## Vehicle List

**Route:** /

**Purpose:** Show all vehicles and serve as the main entry point to the app.

**Displays:**
- Page title and primary navigation
- List of all vehicles
- For each vehicle: Year, Make, Model, License Plate if present, VIN if present, and current Mileage if present
- Empty state message if no vehicles exist yet

**Actions:**
- Create Vehicle → `/vehicles/new`
- Select Vehicle → `/vehicles/:vehicleId`
- Delete Vehicle → removes the vehicle, its maintenance records, and linked maintenance parts, then refreshes `/`
- Go to Parts Library → `/parts`

## Create Vehicle

**Route:** /vehicles/new

**Purpose:** Create a new vehicle record.

**Displays:**
- Vehicle form
- Fields: VIN, License Plate, Year, Make, Model, Purchase Date, Purchase Price, Mileage at Purchase, Mileage
- Make dropdown populated from reference data
- Model dropdown populated after a Make is selected

**Actions:**
- Save Vehicle → creates the vehicle and goes to `/vehicles/:vehicleId`
- Cancel → `/`

## Vehicle Detail

**Route:** /vehicles/:vehicleId

**Purpose:** Show one vehicle and all maintenance recorded for it.

**Displays:**
- Vehicle summary: Year, Make, Model, VIN, License Plate, Purchase Date, Purchase Price, Mileage at Purchase, Mileage
- Maintenance section listing the vehicle's maintenance records
- For each maintenance record: Description, Service Date, Mileage, and number of linked parts
- Empty state message if the vehicle has no maintenance records yet

**Actions:**
- Edit Vehicle → `/vehicles/:vehicleId/edit`
- Delete Vehicle → removes the vehicle, its maintenance records, and linked maintenance parts, then goes to `/`
- Add Maintenance → `/vehicles/:vehicleId/maintenance/new`
- Open Maintenance Record → `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Back to Vehicles → `/`
- Go to Parts Library → `/parts`

## Edit Vehicle

**Route:** /vehicles/:vehicleId/edit

**Purpose:** Update an existing vehicle record.

**Displays:**
- Vehicle form prefilled with the current vehicle data
- Fields: VIN, License Plate, Year, Make, Model, Purchase Date, Purchase Price, Mileage at Purchase, Mileage
- Make dropdown populated from reference data
- Model dropdown populated for the selected Make

**Actions:**
- Save Changes → updates the vehicle and goes to `/vehicles/:vehicleId`
- Cancel → `/vehicles/:vehicleId`

## Create Maintenance

**Route:** /vehicles/:vehicleId/maintenance/new

**Purpose:** Create a maintenance record for a specific vehicle.

**Displays:**
- Maintenance form
- Vehicle summary for context
- Fields: Description, Mileage, Service Date

**Actions:**
- Save Maintenance → creates the maintenance record and goes to `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Cancel → `/vehicles/:vehicleId`

## Maintenance Detail

**Route:** /vehicles/:vehicleId/maintenance/:maintenanceId

**Purpose:** Show one maintenance record and the parts linked to it.

**Displays:**
- Vehicle summary for context
- Maintenance summary: Description, Mileage, Service Date
- Linked parts table
- For each linked part: Name, Model Number if present, Description, Quantity, Unit Price, and line total
- Aggregate parts cost for the maintenance record
- Empty state message if no parts are linked yet

**Actions:**
- Edit Maintenance → `/vehicles/:vehicleId/maintenance/:maintenanceId/edit`
- Delete Maintenance → removes the maintenance record and its linked maintenance parts, then goes to `/vehicles/:vehicleId`
- Add Part to Maintenance → `/vehicles/:vehicleId/maintenance/:maintenanceId/parts/add`
- Edit Linked Part → `/vehicles/:vehicleId/maintenance/:maintenanceId/parts/:partId/edit`
- Remove Linked Part → removes the maintenance-part link and stays on `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Back to Vehicle → `/vehicles/:vehicleId`
- Go to Parts Library → `/parts`

## Edit Maintenance

**Route:** /vehicles/:vehicleId/maintenance/:maintenanceId/edit

**Purpose:** Update an existing maintenance record.

**Displays:**
- Maintenance form prefilled with the current record
- Vehicle summary for context
- Fields: Description, Mileage, Service Date

**Actions:**
- Save Changes → updates the maintenance record and goes to `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Cancel → `/vehicles/:vehicleId/maintenance/:maintenanceId`

## Add Part to Maintenance

**Route:** /vehicles/:vehicleId/maintenance/:maintenanceId/parts/add

**Purpose:** Link an existing reusable part to a maintenance record.

**Displays:**
- Vehicle and maintenance summary for context
- Selectable list of existing parts from the parts library
- For each part: Name, Model Number if present, Description
- Fields for the selected part link: Quantity and Unit Price
- Empty state message if no reusable parts exist yet

**Actions:**
- Save Linked Part → creates the maintenance-part link and goes to `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Go to Create Part → `/parts/new`
- Cancel → `/vehicles/:vehicleId/maintenance/:maintenanceId`

## Edit Linked Part

**Route:** /vehicles/:vehicleId/maintenance/:maintenanceId/parts/:partId/edit

**Purpose:** Update quantity and unit price for a part already linked to a maintenance record.

**Displays:**
- Vehicle and maintenance summary for context
- Part summary: Name, Model Number, Description
- Fields: Quantity and Unit Price

**Actions:**
- Save Changes → updates the maintenance-part link and goes to `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Remove Linked Part → deletes the maintenance-part link and goes to `/vehicles/:vehicleId/maintenance/:maintenanceId`
- Cancel → `/vehicles/:vehicleId/maintenance/:maintenanceId`

## Parts Library

**Route:** /parts

**Purpose:** Manage the reusable catalog of parts.

**Displays:**
- Page title and primary navigation
- List of all parts
- For each part: Name, Model Number, and Description
- Empty state message if no parts exist yet

**Actions:**
- Create Part → `/parts/new`
- Edit Part → `/parts/:partId/edit`
- Delete Part → removes the part and any linked maintenance-part records, then refreshes `/parts`
- Back to Vehicles → `/`

## Create Part

**Route:** /parts/new

**Purpose:** Create a reusable part that can later be linked to maintenance records.

**Displays:**
- Part form
- Fields: Name, Model Number, Description

**Actions:**
- Save Part → creates the part and goes to `/parts`
- Cancel → `/parts`

## Edit Part

**Route:** /parts/:partId/edit

**Purpose:** Update an existing reusable part.

**Displays:**
- Part form prefilled with the current part data
- Fields: Name, Model Number, Description

**Actions:**
- Save Changes → updates the part and goes to `/parts`
- Delete Part → removes the part and any linked maintenance-part records, then goes to `/parts`
- Cancel → `/parts`
