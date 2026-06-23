import type { Part, PartRequest } from '../types/part';
import { parseApiError } from '../utils/parseApiError';

const baseUrl = import.meta.env.VITE_API_URL;

export async function getParts(): Promise<Part[]> {
  const response = await fetch(`${baseUrl}/api/parts`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function getPart(partId: string): Promise<Part> {
  const response = await fetch(`${baseUrl}/api/parts/${partId}`);
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
  return response.json();
}

export async function createPart(request: PartRequest): Promise<Part> {
  const response = await fetch(`${baseUrl}/api/parts`, {
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

export async function updatePart(partId: string, request: PartRequest): Promise<Part> {
  const response = await fetch(`${baseUrl}/api/parts/${partId}`, {
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

export async function deletePart(partId: string): Promise<void> {
  const response = await fetch(`${baseUrl}/api/parts/${partId}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    const message = await parseApiError(response);
    throw new Error(message);
  }
}
