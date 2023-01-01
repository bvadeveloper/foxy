FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /sdk

COPY . .
RUN dotnet restore "services/Consumers/Platform.Consumer.Scanner/Platform.Consumer.Scanner.csproj"
RUN dotnet build "services/Consumers/Platform.Consumer.Scanner/Platform.Consumer.Scanner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "services/Consumers/Platform.Consumer.Scanner/Platform.Consumer.Scanner.csproj" -c Release -o /app/publish

FROM vsp/toolkit:0.1 AS runtime
WORKDIR /app

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .

# expose 80 port for healthchecks
EXPOSE 80

RUN dotnet --info

ENTRYPOINT ["dotnet", "Platform.Consumer.Scanner.dll"]
