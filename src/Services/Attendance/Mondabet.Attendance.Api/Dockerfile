FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Attendance/Mondabet.Attendance.Api/Mondabet.Attendance.Api.csproj", "src/Services/Attendance/Mondabet.Attendance.Api/"]
COPY ["src/Services/Attendance/Mondabet.Attendance.Application/Mondabet.Attendance.Application.csproj", "src/Services/Attendance/Mondabet.Attendance.Application/"]
COPY ["src/Services/Attendance/Mondabet.Attendance.Domain/Mondabet.Attendance.Domain.csproj", "src/Services/Attendance/Mondabet.Attendance.Domain/"]
COPY ["src/Services/Attendance/Mondabet.Attendance.Infrastructure/Mondabet.Attendance.Infrastructure.csproj", "src/Services/Attendance/Mondabet.Attendance.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Attendance/Mondabet.Attendance.Api/Mondabet.Attendance.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Attendance/Mondabet.Attendance.Api/Mondabet.Attendance.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Attendance.Api.dll"]
