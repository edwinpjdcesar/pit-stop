import type { Vehicle } from '../types/vehicle';
import type { VehicleRequest } from '../types/vehicleRequest';
import { parseApiError } from '../utils/parseApiError';

const baseUrl = import.meta.env.VITE_API_URL;

export async function getVehicles(): Promise<Vehicle[]> {
  const response = await fetch(`${baseUrl}/api/vehicles`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function getVehicle(vehicleId: string): Promise<Vehicle> {
  const response = await fetch(`${baseUrl}/api/vehicles/${vehicleId}`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function createVehicle(request: VehicleRequest): Promise<Vehicle> {
  const response = await fetch(`${baseUrl}/api/vehicles`, {
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

export async function updateVehicle(vehicleId: string, request: VehicleRequest): Promise<Vehicle> {
  const response = await fetch(`${baseUrl}/api/vehicles/${vehicleId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  });
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function deleteVehicle(vehicleId: string): Promise<void> {
  const response = await fetch(`${baseUrl}/api/vehicles/${vehicleId}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
}
