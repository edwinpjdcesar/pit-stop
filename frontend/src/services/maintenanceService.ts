import type { MaintenanceRecord } from '../types/maintenance';
import type { MaintenancePart } from '../types/maintenancePart';
import { parseApiError } from '../utils/parseApiError';

type MaintenancePartRequest = { quantity: number; unitPrice: number };

const baseUrl = import.meta.env.VITE_API_URL;

export async function getMaintenanceRecords(vehicleId: string): Promise<MaintenanceRecord[]> {
  const response = await fetch(`${baseUrl}/api/vehicles/${vehicleId}/maintenance`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function createMaintenanceRecord(
  vehicleId: string,
  request: { description: string; mileage: number; serviceDate: string }
): Promise<MaintenanceRecord> {
  const response = await fetch(`${baseUrl}/api/vehicles/${vehicleId}/maintenance`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function updateMaintenanceRecord(
  vehicleId: string,
  maintenanceId: string,
  request: { description: string; mileage: number; serviceDate: string }
): Promise<MaintenanceRecord> {
  const response = await fetch(
    `${baseUrl}/api/vehicles/${vehicleId}/maintenance/${maintenanceId}`,
    {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    }
  );
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function deleteMaintenanceRecord(
  vehicleId: string,
  maintenanceId: string
): Promise<void> {
  const response = await fetch(
    `${baseUrl}/api/vehicles/${vehicleId}/maintenance/${maintenanceId}`,
    { method: 'DELETE' }
  );
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
}

export async function addMaintenancePart(
  vehicleId: string,
  maintenanceId: string,
  partId: string,
  request: MaintenancePartRequest
): Promise<MaintenancePart> {
  const response = await fetch(
    `${baseUrl}/api/vehicles/${vehicleId}/maintenance/${maintenanceId}/parts/${partId}`,
    {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    }
  );
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function updateMaintenancePart(
  vehicleId: string,
  maintenanceId: string,
  partId: string,
  request: MaintenancePartRequest
): Promise<MaintenancePart> {
  const response = await fetch(
    `${baseUrl}/api/vehicles/${vehicleId}/maintenance/${maintenanceId}/parts/${partId}`,
    {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(request),
    }
  );
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function removeMaintenancePart(
  vehicleId: string,
  maintenanceId: string,
  partId: string
): Promise<void> {
  const response = await fetch(
    `${baseUrl}/api/vehicles/${vehicleId}/maintenance/${maintenanceId}/parts/${partId}`,
    { method: 'DELETE' }
  );
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
}
