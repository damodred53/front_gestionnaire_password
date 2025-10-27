FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["front_gestionnaire_password/front_gestionnaire_password.csproj", "front_gestionnaire_password/"]
RUN dotnet restore "front_gestionnaire_password/front_gestionnaire_password.csproj"
COPY . .
WORKDIR "/src/front_gestionnaire_password"
RUN dotnet build "./front_gestionnaire_password.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./front_gestionnaire_password.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "front_gestionnaire_password.dll"]
