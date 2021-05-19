FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ProductionManagementSystem.API/ProductionManagementSystem.API.csproj", "ProductionManagementSystem.API/"]
RUN dotnet restore "ProductionManagementSystem.API/ProductionManagementSystem.API.csproj"
COPY . .
WORKDIR "/src/ProductionManagementSystem.API"
RUN dotnet build "ProductionManagementSystem.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductionManagementSystem.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductionManagementSystem.API.dll"]
