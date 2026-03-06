FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["Services/Payment/Payment.API/Payment.API.csproj", "Services/Payment/Payment.API/"]
COPY ["BuildingBlocks/BuildingBlocks.Exceptions/BuildingBlocks.Exceptions.csproj", "BuildingBlocks/BuildingBlocks.Exceptions/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "Services/Payment/Payment.API/Payment.API.csproj"
COPY . .
WORKDIR "/src/Services/Payment/Payment.API"
RUN dotnet build "Payment.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.API.dll"]
