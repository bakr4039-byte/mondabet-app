// Placeholder value only - the Docker build for this portal (see
// src/portals/super-admin/Dockerfile) overwrites this file's apiUrl with the real
// API_URL build argument before running `ng build --configuration production`,
// so this literal value is never actually shipped as-is. This file already existed
// but nothing in angular.json told the production build to actually use it (see
// the new "fileReplacements" entry below) - so it was dead code until now.
export const environment = {
  production: true,
  apiUrl: 'https://api.example.com/api/v1',
};
