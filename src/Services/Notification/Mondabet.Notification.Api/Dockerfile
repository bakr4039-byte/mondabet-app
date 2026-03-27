FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Notification/Mondabet.Notification.Api/Mondabet.Notification.Api.csproj", "src/Services/Notification/Mondabet.Notification.Api/"]
COPY ["src/Services/Notification/Mondabet.Notification.Application/Mondabet.Notification.Application.csproj", "src/Services/Notification/Mondabet.Notification.Application/"]
COPY ["src/Services/Notification/Mondabet.Notification.Infrastructure/Mondabet.Notification.Infrastructure.csproj", "src/Services/Notification/Mondabet.Notification.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Notification/Mondabet.Notification.Api/Mondabet.Notification.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Notification/Mondabet.Notification.Api/Mondabet.Notification.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Notification.Api.dll"]
