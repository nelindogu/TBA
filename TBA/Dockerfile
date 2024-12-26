# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .

# Expose the default ASP.NET Core port
EXPOSE 8080

# Set the entry point for the application
ENTRYPOINT ["dotnet", "TBA.dll"]

