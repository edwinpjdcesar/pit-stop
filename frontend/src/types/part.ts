export type Part = {
  partId: string;
  name: string;
  modelNumber: string | null;
  description: string;
};

export type PartRequest = {
  name: string;
  modelNumber: string | null;
  description: string;
};
