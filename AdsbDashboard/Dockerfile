#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# https://hub.docker.com/_/microsoft-dotnet-aspnet/


#ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm64v8 # Raspberry Pi
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0 


FROM ${DOTNET_RUNTIME} AS base
USER app
WORKDIR /app

EXPOSE 5000
EXPOSE 5001

FROM ${DOTNET_SDK} AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["AdsbDashboard/AdsbDashboard.csproj", "AdsbDashboard/"]
RUN dotnet restore "./AdsbDashboard/./AdsbDashboard.csproj"
COPY . .
WORKDIR "/src/AdsbDashboard"
RUN dotnet build "./AdsbDashboard.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdsbDashboard.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdsbDashboard.dll"]

# Run command from folder up
# windows: 
# docker build . -t AdsbDashboard -f .\AdsbDashboard\Dockerfile
# linux:
# docker build . -t AdsbDashboard -f AdsbDashboard/Dockerfile
# docker build ../ -t AdsbDashboard -f Dockerfile
