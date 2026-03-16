FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["Services/Notification/Notification.API/Notification.API.csproj", "Services/Notification/Notification.API/"]
COPY ["BuildingBlocks/BuildingBlocks.Exceptions/BuildingBlocks.Exceptions.csproj", "BuildingBlocks/BuildingBlocks.Exceptions/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "Services/Notification/Notification.API/Notification.API.csproj"
COPY . .
WORKDIR "/src/Services/Notification/Notification.API"
RUN dotnet build "Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notification.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.API.dll"]
