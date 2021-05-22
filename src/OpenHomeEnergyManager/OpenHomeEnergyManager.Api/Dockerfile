#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["OpenHomeEnergyManager.Api/OpenHomeEnergyManager.Api.csproj", "OpenHomeEnergyManager.Api/"]
COPY ["OpenHomeEnergyManager.Domain/OpenHomeEnergyManager.Domain.csproj", "OpenHomeEnergyManager.Domain/"]
COPY ["OpenHomeEnergyManager.Infrastructure/OpenHomeEnergyManager.Infrastructure.csproj", "OpenHomeEnergyManager.Infrastructure/"]
RUN dotnet restore "OpenHomeEnergyManager.Api/OpenHomeEnergyManager.Api.csproj"
COPY . .
WORKDIR "/src/OpenHomeEnergyManager.Api"
RUN dotnet build "OpenHomeEnergyManager.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenHomeEnergyManager.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenHomeEnergyManager.Api.dll"]
