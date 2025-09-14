FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Nuget.Config .

ARG CRT_PATH

COPY local.devspot.tech.pem /usr/local/share/ca-certificates/devspot-rootCA.crt
RUN update-ca-certificates

COPY ["SaxsSpot.Orchestrator.Host/SaxsSpot.Orchestrator.Host.csproj", "SaxsSpot.Orchestrator.Host/"]
COPY ["SaxsSpot.Orchestrator.Application/SaxsSpot.Orchestrator.Application.csproj", "SaxsSpot.Orchestrator.Application/"]
COPY ["SaxsSpot.Orchestrator.Domain/SaxsSpot.Orchestrator.Domain.csproj", "SaxsSpot.Orchestrator.Domain/"]
COPY ["SaxsSpot.Orchestrator.Contracts/SaxsSpot.Orchestrator.Contracts.csproj", "SaxsSpot.Orchestrator.Contracts/"]
COPY ["SaxsSpot.Orchestrator.Infrastructure/SaxsSpot.Orchestrator.Infrastructure.csproj", "SaxsSpot.Orchestrator.Infrastructure/"]
COPY ["SaxsSpot.Orchestrator.Kafka/SaxsSpot.Orchestrator.Kafka.csproj", "SaxsSpot.Orchestrator.Kafka/"]
RUN dotnet dev-certs https --trust || true

RUN dotnet restore "SaxsSpot.Orchestrator.Host/SaxsSpot.Orchestrator.Host.csproj" --configfile Nuget.Config --verbosity detailed
COPY . .
WORKDIR "/src/SaxsSpot.Orchestrator.Host"
RUN dotnet build "SaxsSpot.Orchestrator.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SaxsSpot.Orchestrator.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaxsSpot.Orchestrator.Host.dll"]
#docker build --build-arg CRT_PATH="local.devspot.tech.pem" . -t "jordenndev/saxsspot-orchestrator-service"