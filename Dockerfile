#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# https://hub.docker.com/_/microsoft-dotnet-aspnet/
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim-arm64v8 AS base
USER app
WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AdsbMudBlazor/AdsbMudBlazor.csproj", "AdsbMudBlazor/"]
RUN dotnet restore "./AdsbMudBlazor/./AdsbMudBlazor.csproj"
COPY . .
WORKDIR "/src/AdsbMudBlazor"
RUN dotnet build "./AdsbMudBlazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdsbMudBlazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdsbMudBlazor.dll"]

# Run command from folder up
# windows: 
# docker build . -t adsbmudblazor -f .\AdsbMudBlazor\Dockerfile
# linux:
# docker build . -t adsbmudblazor -f AdsbMudBlazor/Dockerfile
# docker build ../ -t adsbmudblazor -f Dockerfile
