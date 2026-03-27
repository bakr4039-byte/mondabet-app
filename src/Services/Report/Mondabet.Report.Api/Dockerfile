FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Report/Mondabet.Report.Api/Mondabet.Report.Api.csproj", "src/Services/Report/Mondabet.Report.Api/"]
COPY ["src/Services/Report/Mondabet.Report.Application/Mondabet.Report.Application.csproj", "src/Services/Report/Mondabet.Report.Application/"]
COPY ["src/Services/Report/Mondabet.Report.Infrastructure/Mondabet.Report.Infrastructure.csproj", "src/Services/Report/Mondabet.Report.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Report/Mondabet.Report.Api/Mondabet.Report.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Report/Mondabet.Report.Api/Mondabet.Report.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Report.Api.dll"]
