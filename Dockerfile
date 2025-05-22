# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything into the container
COPY . .

# Restore dependencies
RUN dotnet restore "./Cafe_Web_App/Cafe_Web_App.csproj" --disable-parallel

# Publish the application in release mode to the /app directory
RUN dotnet publish "./Cafe_Web_App/Cafe_Web_App.csproj" -c Release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app

# Copy the published output from the build stage to the serve stage
COPY --from=build /app .

# Set the ASP.NET Core URLs to listen on port 5000
ENV ASPNETCORE_URLS=http://+:5000

# Expose the application port
EXPOSE 5000

# Define the entry point for the container to run your application
ENTRYPOINT ["dotnet", "Cafe_Web_App.dll"]
