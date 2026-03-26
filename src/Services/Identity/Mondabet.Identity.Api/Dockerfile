FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Identity/Mondabet.Identity.Api/Mondabet.Identity.Api.csproj", "src/Services/Identity/Mondabet.Identity.Api/"]
COPY ["src/Services/Identity/Mondabet.Identity.Application/Mondabet.Identity.Application.csproj", "src/Services/Identity/Mondabet.Identity.Application/"]
COPY ["src/Services/Identity/Mondabet.Identity.Domain/Mondabet.Identity.Domain.csproj", "src/Services/Identity/Mondabet.Identity.Domain/"]
COPY ["src/Services/Identity/Mondabet.Identity.Infrastructure/Mondabet.Identity.Infrastructure.csproj", "src/Services/Identity/Mondabet.Identity.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Identity/Mondabet.Identity.Api/Mondabet.Identity.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Identity/Mondabet.Identity.Api/Mondabet.Identity.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Identity.Api.dll"]
