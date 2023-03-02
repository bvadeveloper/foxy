FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /sdk

COPY . .
RUN dotnet restore "services/Bots/Platform.Telegram.Bot/Platform.Telegram.Bot.csproj"
RUN dotnet build "services/Bots/Platform.Telegram.Bot/Platform.Telegram.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "services/Bots/Platform.Telegram.Bot/Platform.Telegram.Bot.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .

# expose 80 port for healthchecks
EXPOSE 80

ENTRYPOINT ["dotnet", "Platform.Telegram.Bot.dll"]
