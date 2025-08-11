FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagem de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas o .csproj primeiro
COPY ../OmniSuite.API/OmniSuite.API.csproj ./OmniSuite.API/
COPY ../OmniSuite.Application/OmniSuite.Application.csproj ./OmniSuite.Application/
COPY ../OmniSuite.Domain/OmniSuite.Domain.csproj ./OmniSuite.Domain/
COPY ../OmniSuite.Infrastructure/OmniSuite.Infrastructure.csproj ./OmniSuite.Infrastructure/
COPY ../OmniSuite.Persistence/OmniSuite.Persistence.csproj ./OmniSuite.Persistence/

# Restaura dependências
RUN dotnet restore /src/OmniSuite.API/OmniSuite.API.csproj

# Copia todo o código restante
COPY ../OmniSuite.API ./OmniSuite.API/
COPY ../OmniSuite.Application ./OmniSuite.Application/
COPY ../OmniSuite.Domain ./OmniSuite.Domain/
COPY ../OmniSuite.Infrastructure ./OmniSuite.Infrastructure/
COPY ../OmniSuite.Persistence ./OmniSuite.Persistence/

# Build do projeto
WORKDIR /src/OmniSuite.API
RUN dotnet build "OmniSuite.API.csproj" -c Release -o /app/build

# Publica o projeto
FROM build AS publish
RUN dotnet publish "OmniSuite.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OmniSuite.API.dll"]