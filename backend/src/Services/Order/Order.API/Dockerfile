FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["Services/Order/Order.API/Order.API.csproj", "Services/Order/Order.API/"]
COPY ["Services/Order/Order.Application/Order.Application.csproj", "Services/Order/Order.Application/"]
COPY ["Services/Order/Order.Domain/Order.Domain.csproj", "Services/Order/Order.Domain/"]
COPY ["Services/Order/Order.Infrastructure/Order.Infrastructure.csproj", "Services/Order/Order.Infrastructure/"]
COPY ["BuildingBlocks/BuildingBlocks.Exceptions/BuildingBlocks.Exceptions.csproj", "BuildingBlocks/BuildingBlocks.Exceptions/"]
COPY ["BuildingBlocks/BuildingBlocks.Behaviors/BuildingBlocks.Behaviors.csproj", "BuildingBlocks/BuildingBlocks.Behaviors/"]
COPY ["BuildingBlocks/BuildingBlocks.Messaging/BuildingBlocks.Messaging.csproj", "BuildingBlocks/BuildingBlocks.Messaging/"]
RUN dotnet restore "Services/Order/Order.API/Order.API.csproj"
COPY . .
WORKDIR "/src/Services/Order/Order.API"
RUN dotnet build "Order.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.API.dll"]
