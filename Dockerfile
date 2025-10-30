# ===========================================================
# 🏗️ Etapa 1: Build de la aplicación .NET 9
# ===========================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar los archivos de proyecto (solo los .csproj)
COPY ["Web/Web.csproj", "Web/"]
COPY ["Business/Business.csproj", "Business/"]
COPY ["Data/Data.csproj", "Data/"]
COPY ["Entity/Entity.csproj", "Entity/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]

# Restaurar dependencias
RUN dotnet restore "Web/Web.csproj"

# Copiar el resto del código fuente
COPY . .

# Cambiar al directorio del proyecto Web
WORKDIR "/src/Web"

# Compilar y publicar en modo Release
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

# ===========================================================
# 🚀 Etapa 2: Imagen final (runtime)
# ===========================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la etapa anterior
COPY --from=build /app/publish .

# Exponer el puerto (coherente con ASPNETCORE_URLS)
EXPOSE 8080

# Establecer el punto de entrada
ENTRYPOINT ["dotnet", "Web.dll"]
