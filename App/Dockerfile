FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["gluetun2qB.csproj", "./"]
RUN dotnet restore "gluetun2qB.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "gluetun2qB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gluetun2qB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gluetun2qB.dll"]
