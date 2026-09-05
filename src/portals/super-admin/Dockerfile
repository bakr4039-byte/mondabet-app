# Builds and serves the Super-Admin portal as a static site behind nginx.
# API_URL is baked in at build time (Angular has no runtime env config here), so this
# image must be rebuilt whenever the API's public URL changes - see nginx.conf for the
# SPA fallback that makes client-side routing work behind a reverse proxy.
FROM node:20-alpine AS build
WORKDIR /app
ARG API_URL=https://api.example.com/api/v1

COPY package*.json ./
RUN npm ci

COPY . .
RUN printf "export const environment = {\n  production: true,\n  apiUrl: '%s',\n};\n" "$API_URL" \
    > src/environments/environment.prod.ts
RUN npx ng build --configuration production

FROM nginx:1.27-alpine AS final
COPY --from=build /app/dist/super-admin/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
