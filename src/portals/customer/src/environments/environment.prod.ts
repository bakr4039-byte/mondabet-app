// Placeholder value only - the Docker build for this portal (see
// src/portals/customer/Dockerfile) overwrites this file's apiUrl with the real
// API_URL build argument before running `ng build --configuration production`,
// so this literal value is never actually shipped as-is.
export const environment = {
  production: true,
  apiUrl: 'https://api.example.com/api/v1',
};
