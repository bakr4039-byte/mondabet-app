FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Tenant/Mondabet.Tenant.Api/Mondabet.Tenant.Api.csproj", "src/Services/Tenant/Mondabet.Tenant.Api/"]
COPY ["src/Services/Tenant/Mondabet.Tenant.Application/Mondabet.Tenant.Application.csproj", "src/Services/Tenant/Mondabet.Tenant.Application/"]
COPY ["src/Services/Tenant/Mondabet.Tenant.Domain/Mondabet.Tenant.Domain.csproj", "src/Services/Tenant/Mondabet.Tenant.Domain/"]
COPY ["src/Services/Tenant/Mondabet.Tenant.Infrastructure/Mondabet.Tenant.Infrastructure.csproj", "src/Services/Tenant/Mondabet.Tenant.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Tenant/Mondabet.Tenant.Api/Mondabet.Tenant.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Tenant/Mondabet.Tenant.Api/Mondabet.Tenant.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Tenant.Api.dll"]
