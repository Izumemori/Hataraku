#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
RUN ls -la
COPY ["nuget.config", "src/Hataraku.Bot/"]
COPY ["src/Hataraku.Bot/Hataraku.Bot.csproj", "src/Hataraku.Bot/"]
RUN dotnet restore "src/Hataraku.Bot/Hataraku.Bot.csproj"
COPY . .
WORKDIR "/src/src/Hataraku.Bot"
RUN dotnet build "Hataraku.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hataraku.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hataraku.Bot.dll"]
