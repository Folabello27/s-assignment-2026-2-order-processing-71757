FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SportsStore.OrderApi/", "SportsStore.OrderApi/"]
COPY ["SportsStore.Core/", "SportsStore.Core/"]
COPY ["SportsStore.Infrastructure/", "SportsStore.Infrastructure/"]
RUN dotnet restore "SportsStore.OrderApi/SportsStore.OrderApi.csproj"
RUN dotnet build "SportsStore.OrderApi/SportsStore.OrderApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SportsStore.OrderApi/SportsStore.OrderApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SportsStore.OrderApi.dll"]
