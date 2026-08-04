# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution
COPY . .

# Restore (pointing to the correct 'API' directory)
RUN dotnet restore API/API.csproj

# Publish
RUN dotnet publish API/API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProjectPlanner.API.dll"]