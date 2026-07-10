FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy project files first for layer caching
COPY EstocaFacil.Domain/EstocaFacil.Domain.csproj EstocaFacil.Domain/
COPY EstocaFacil.Application/EstocaFacil.Application.csproj EstocaFacil.Application/
COPY EstocaFacil.Infrastructure/EstocaFacil.Infrastructure.csproj EstocaFacil.Infrastructure/
COPY EstocaFacil.API/EstocaFacil.API.csproj EstocaFacil.API/

# Restore dependencies
RUN dotnet restore EstocaFacil.API/EstocaFacil.API.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish EstocaFacil.API/EstocaFacil.API.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-3000}
EXPOSE 3000

ENTRYPOINT ["dotnet", "EstocaFacil.API.dll"]
