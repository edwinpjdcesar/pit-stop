import type { MaintenancePart } from './maintenancePart';

export type MaintenanceRecord = {
  maintenanceId: string;
  vehicleId: string;
  description: string;
  mileage: number;
  serviceDate: string;
  maintenanceParts: MaintenancePart[];
};
