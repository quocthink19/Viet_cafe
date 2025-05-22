# Use .NET 8.0 SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore "./Cafe_Web_App/Cafe_Web_App.csproj" --disable-parallel

# Publish the application in release mode to the /app directory
RUN dotnet publish "./Cafe_Web_App/Cafe_Web_App.csproj" -c Release -o /app --no-restore

# Use ASP.NET 8.0 runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Cafe_Web_App.dll"]