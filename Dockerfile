# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution
COPY . .

# Restore
RUN dotnet restore ProjectPlanner.Api/ProjectPlanner.Api.csproj

# Publish
RUN dotnet publish ProjectPlanner.Api/ProjectPlanner.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProjectPlanner.Api.dll"]