# docker build -f .\ElasticLoggingWeb\Dockerfile -t elastic-logging:latest .
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ElasticLoggingWeb/ElasticLoggingWeb.csproj", "ElasticLoggingWeb/"]
RUN dotnet restore "ElasticLoggingWeb/ElasticLoggingWeb.csproj"
COPY . .
WORKDIR "/src/ElasticLoggingWeb"
RUN dotnet build "ElasticLoggingWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ElasticLoggingWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ElasticLoggingWeb.dll"]
