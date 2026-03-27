FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Workflow/Mondabet.Workflow.Api/Mondabet.Workflow.Api.csproj", "src/Services/Workflow/Mondabet.Workflow.Api/"]
COPY ["src/Services/Workflow/Mondabet.Workflow.Application/Mondabet.Workflow.Application.csproj", "src/Services/Workflow/Mondabet.Workflow.Application/"]
COPY ["src/Services/Workflow/Mondabet.Workflow.Domain/Mondabet.Workflow.Domain.csproj", "src/Services/Workflow/Mondabet.Workflow.Domain/"]
COPY ["src/Services/Workflow/Mondabet.Workflow.Infrastructure/Mondabet.Workflow.Infrastructure.csproj", "src/Services/Workflow/Mondabet.Workflow.Infrastructure/"]
COPY ["src/Shared/Mondabet.Shared/Mondabet.Shared.csproj", "src/Shared/Mondabet.Shared/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/Services/Workflow/Mondabet.Workflow.Api/Mondabet.Workflow.Api.csproj"
COPY . .
RUN dotnet publish "src/Services/Workflow/Mondabet.Workflow.Api/Mondabet.Workflow.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Mondabet.Workflow.Api.dll"]
