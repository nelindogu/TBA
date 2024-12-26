# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory for build stage
WORKDIR /src

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining source code
COPY . . 

# Build and publish the application
RUN dotnet publish -c Release -o /app


# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory for runtime stage
WORKDIR /app

# Copy the application from the build stage
COPY --from=build /app .

# Expose the application port
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "TBA.dll"]
