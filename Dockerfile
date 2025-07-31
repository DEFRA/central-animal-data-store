FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8080
EXPOSE 8081
RUN apt update && \
    apt install curl -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "CAS.Api/CAS.Api.csproj"
RUN dotnet test CAS.Test
RUN dotnet build "CAS.Api/CAS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:UseAppHost=false




FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CAS.Api/CAS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8085
ENTRYPOINT ["dotnet", "CAS.Api.dll"]
