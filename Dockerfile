# ===========================================================
# 🏗️ Etapa 1: Build de la aplicación .NET 9
# ===========================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Web/Web.csproj", "Web/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Entity/Entity.csproj", "Entity/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]

RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR "/src/Web"

# ⚡ Publicar y asegurar que los appsettings se copien
RUN dotnet publish "Web.csproj" -c Release -o /app/publish \
    && cp appsettings*.json /app/publish/ || true

# ===========================================================
# 🚀 Etapa 2: Imagen final (runtime)
# ===========================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]