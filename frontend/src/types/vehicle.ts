import type { Make } from './make';
import type { Model } from './model';

export type Vehicle = {
  vehicleId: string;
  vin: string | null;
  licensePlate: string | null;
  year: number;
  make: Make;
  model: Model;
  purchaseDate: string | null;
  purchasePrice: number | null;
  mileageAtPurchase: number | null;
  mileage: number | null;
};
