# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

# Copy project files and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the app and publish it
COPY . ./
RUN dotnet publish -c Release -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app .

# Expose port for the app
EXPOSE 8080

# Set the entry point for the app
ENTRYPOINT ["dotnet", "TBA.dll"]
