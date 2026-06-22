import type { Make } from '../types/make';
import type { Model } from '../types/model';
import { parseApiError } from '../utils/parseApiError';

const baseUrl = import.meta.env.VITE_API_URL;

export async function getMakes(): Promise<Make[]> {
  const response = await fetch(`${baseUrl}/api/makes`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function getModels(makeId: number): Promise<Model[]> {
  const response = await fetch(`${baseUrl}/api/makes/${makeId}/models`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}
