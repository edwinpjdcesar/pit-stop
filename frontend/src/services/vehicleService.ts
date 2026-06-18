import type { Vehicle } from '../types/vehicle';

const baseUrl = import.meta.env.VITE_API_URL;

export async function getVehicles(): Promise<Vehicle[]> {
  try {
    const response = await fetch(`${baseUrl}/api/vehicles`);

    if (!response.ok) {
      throw new Error(`Failed to fetch vehicles: ${response.status}`);
    }

    return response.json();
  } catch (error) {
    console.error('[vehicleService] getVehicles failed:', error);
    throw error;
  }
}
