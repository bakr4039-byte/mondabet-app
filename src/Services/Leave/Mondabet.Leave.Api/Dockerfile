FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Leave/Mondabet.Leave.Api/Mondabet.Leave.Api.csproj", "src/Services/Leave/Mondabet.Leave.Api/"]
COPY ["src/Services/Leave/Mondabet.Leave.Application/Mondabet.Leave.Application.csproj", "src/Services/Leave/Mondabet.Leave.Application/"]
COPY ["src/Services/Leave/Mondabet.Leave.Domain/Mondabet.Leave.Domain.csproj", "src/Services/Leave/Mondabet.Leave.Domain/"]
COPY ["src/Services/Leave/Mondabet.Leave.Infrastructure/Mondabet.Leave.Infrastructure.csproj", "src/Services/Leave/Mondabet.Leave.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Leave/Mondabet.Leave.Api/Mondabet.Leave.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Leave/Mondabet.Leave.Api/Mondabet.Leave.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Leave.Api.dll"]
