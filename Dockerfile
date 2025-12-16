# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/SomosRentWi.Api/SomosRentWi.Api.csproj", "SomosRentWi.Api/"]
COPY ["src/SomosRentWi.Application/SomosRentWi.Application.csproj", "SomosRentWi.Application/"]
COPY ["src/SomosRentWi.Domain/SomosRentWi.Domain.csproj", "SomosRentWi.Domain/"]
COPY ["src/SomosRentWi.Infrastructure/SomosRentWi.Infrastructure.csproj", "SomosRentWi.Infrastructure/"]

RUN dotnet restore "SomosRentWi.Api/SomosRentWi.Api.csproj"

# Copy everything else and build
COPY src/ .
WORKDIR "/src/SomosRentWi.Api"
RUN dotnet build "SomosRentWi.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SomosRentWi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
COPY entrypoint.sh /app/entrypoint.sh

# Make entrypoint executable
RUN chmod +x /app/entrypoint.sh

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["/app/entrypoint.sh"]
