#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic-arm32v7 AS base
WORKDIR /app
RUN DEBIAN_FRONTEND="noninteractive" apt update && apt upgrade -y && apt install docker.io docker-compose -y
#RUN apt update && apt upgrade -y && apt install docker.io docker-compose -y

#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic-arm32v7 AS build
WORKDIR /src
COPY ["BootstrapToAzure.WorkerService/BootstrapToAzure.WorkerService.csproj", "BootstrapToAzure.WorkerService/"]
COPY ["BootstrapToAzure.Business/BootstrapToAzure.Business.csproj", "BootstrapToAzure.Business/"]
COPY ["BootstrapToAzure.Common/BootstrapToAzure.Common.csproj", "BootstrapToAzure.Common/"]
COPY ["BootstrapToAzure.Data/BootstrapToAzure.Data.csproj", "BootstrapToAzure.Data/"]
RUN dotnet restore "BootstrapToAzure.WorkerService/BootstrapToAzure.WorkerService.csproj"
COPY . .
WORKDIR "/src/BootstrapToAzure.WorkerService"
RUN dotnet build "BootstrapToAzure.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BootstrapToAzure.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BootstrapToAzure.WorkerService.dll"]