export type VehicleRequest = {
  makeId: number;
  modelId: number;
  year: number;
  vin?: string | null;
  licensePlate?: string | null;
  purchaseDate?: string | null;
  purchasePrice?: number | null;
  mileageAtPurchase?: number | null;
  mileage?: number | null;
};
