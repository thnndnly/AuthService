# Basis-Image für die Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build-Image mit .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiere und restore die Abhängigkeiten
COPY ["AuthService/AuthService.csproj", "AuthService/"]
RUN dotnet restore "AuthService/AuthService.csproj"

# Kopiere den restlichen Code und baue die Anwendung
COPY . .
WORKDIR "/src/AuthService"
RUN dotnet build "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish-Schritt
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Finales Image mit Runtime
FROM base AS final
WORKDIR /app

# Erstelle das Verzeichnis für die SQLite-Datenbank und setze Schreibrechte
USER root
RUN mkdir -p /var/data
RUN chmod -R 777 /var/data

# Kopiere die vorkonfigurierte Datenbank (falls vorhanden) und setze Zugriffsrechte
# COPY AuthService/app.db /var/data/app.db
# RUN chmod 777 /var/data/app.db

# Optional: Falls keine Datenbank vorhanden, erstelle sie (dieser Befehl hat dann keine Wirkung, wenn die Datei bereits existiert)
# RUN test -f /var/data/app.db || (touch /var/data/app.db && chmod 777 /var/data/app.db)

# Wechsel zurück zum Anwendung-Benutzer
USER $APP_UID

# Kopiere die Anwendung aus dem Publish-Image
COPY --from=publish /app/publish .

# Start der Anwendung
ENTRYPOINT ["dotnet", "AuthService.dll"]
