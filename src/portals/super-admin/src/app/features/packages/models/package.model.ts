export interface Package {
  id: string;
  name: string;
  maxUsers: number;
  priceMonthly: number;
  features: string;
}

export interface PackageCreateDto {
  name: string;
  maxUsers: number;
  priceMonthly: number;
  features: string;
}
