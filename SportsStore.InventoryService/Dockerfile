FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SportsStore.InventoryService/", "SportsStore.InventoryService/"]
COPY ["SportsStore.Core/", "SportsStore.Core/"]
COPY ["SportsStore.Infrastructure/", "SportsStore.Infrastructure/"]
RUN dotnet restore "SportsStore.InventoryService/SportsStore.InventoryService.csproj"
RUN dotnet build "SportsStore.InventoryService/SportsStore.InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportsStore.InventoryService/SportsStore.InventoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportsStore.InventoryService.dll"]
