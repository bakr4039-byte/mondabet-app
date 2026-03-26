FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Employee/Mondabet.Employee.Api/Mondabet.Employee.Api.csproj", "src/Services/Employee/Mondabet.Employee.Api/"]
COPY ["src/Services/Employee/Mondabet.Employee.Application/Mondabet.Employee.Application.csproj", "src/Services/Employee/Mondabet.Employee.Application/"]
COPY ["src/Services/Employee/Mondabet.Employee.Domain/Mondabet.Employee.Domain.csproj", "src/Services/Employee/Mondabet.Employee.Domain/"]
COPY ["src/Services/Employee/Mondabet.Employee.Infrastructure/Mondabet.Employee.Infrastructure.csproj", "src/Services/Employee/Mondabet.Employee.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Employee/Mondabet.Employee.Api/Mondabet.Employee.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Employee/Mondabet.Employee.Api/Mondabet.Employee.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Employee.Api.dll"]
