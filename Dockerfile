#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base

EXPOSE 8005
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:8005

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestServiceBusWrokerApp/TestServiceBusWrokerApp.csproj", "TestServiceBusWrokerApp/"]
RUN dotnet restore "TestServiceBusWrokerApp/TestServiceBusWrokerApp.csproj"
COPY . .
WORKDIR "/src/TestServiceBusWrokerApp"
RUN dotnet build "TestServiceBusWrokerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestServiceBusWrokerApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestServiceBusWrokerApp.dll"]