FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Clarification/Mondabet.Clarification.Api/Mondabet.Clarification.Api.csproj", "src/Services/Clarification/Mondabet.Clarification.Api/"]
COPY ["src/Services/Clarification/Mondabet.Clarification.Application/Mondabet.Clarification.Application.csproj", "src/Services/Clarification/Mondabet.Clarification.Application/"]
COPY ["src/Services/Clarification/Mondabet.Clarification.Domain/Mondabet.Clarification.Domain.csproj", "src/Services/Clarification/Mondabet.Clarification.Domain/"]
COPY ["src/Services/Clarification/Mondabet.Clarification.Infrastructure/Mondabet.Clarification.Infrastructure.csproj", "src/Services/Clarification/Mondabet.Clarification.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Clarification/Mondabet.Clarification.Api/Mondabet.Clarification.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Clarification/Mondabet.Clarification.Api/Mondabet.Clarification.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Clarification.Api.dll"]
