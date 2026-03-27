export interface Tenant {
  id: string;
  name: string;
  code: string;
  adminEmail: string;
  adminMobile: string;
  primaryColor: string;
  secondaryColor: string;
  logoUrl: string;
  isActive: boolean;
  subscriptionEndsAt: string;
  packageId: string;
}

export interface TenantCreateDto {
  name: string;
  code: string;
  adminEmail: string;
  adminMobile: string;
  primaryColor: string;
  secondaryColor: string;
  packageId: string;
  subscriptionEndsAt: string;
}
