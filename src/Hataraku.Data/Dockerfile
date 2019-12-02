#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["nuget.config", "src/Hataraku.Bot/"]
COPY ["src/Hataraku.Data/Hataraku.Data.csproj", "src/Hataraku.Data/"]
RUN dotnet restore "src/Hataraku.Data/Hataraku.Data.csproj"
COPY . .
WORKDIR "/src/src/Hataraku.Data"
RUN dotnet build "Hataraku.Data.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hataraku.Data.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hataraku.Data.dll"]
