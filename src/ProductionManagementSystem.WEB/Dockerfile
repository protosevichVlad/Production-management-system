#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Production management system/ProductionManagementSystem.WEB.csproj", "Production management system/"]
COPY ["ProductionManagementSystem.BLL/ProductionManagementSystem.BLL.csproj", "ProductionManagementSystem.BLL/"]
COPY ["ProductionManagementSystem.DAL/ProductionManagementSystem.DAL.csproj", "ProductionManagementSystem.DAL/"]
RUN dotnet restore "Production management system/ProductionManagementSystem.WEB.csproj"
COPY . .
WORKDIR "/src/Production management system"
RUN dotnet build "ProductionManagementSystem.WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductionManagementSystem.WEB.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductionManagementSystem.WEB.dll"]