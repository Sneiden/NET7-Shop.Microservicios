#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Basket.api/Basket.api.csproj", "Basket.api/"]
RUN dotnet restore "Basket.api/Basket.api.csproj"
COPY . .
WORKDIR "/src/Basket.api"
RUN dotnet build "Basket.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Basket.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.api.dll"]