﻿FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /sdk

ARG PROJECT_PATH="services/Collectors/Platform.Collector.Facebook/Platform.Collector.Facebook.csproj"

COPY . .
RUN dotnet restore $PROJECT_PATH
RUN dotnet build $PROJECT_PATH -c Release -o /app/build

FROM build AS publish
RUN dotnet publish $PROJECT_PATH -c Release -o /app/publish

FROM vsp/toolkit:0.1 AS runtime
WORKDIR /app

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app/publish .

# expose 80 port for healthchecks
EXPOSE 80

ENTRYPOINT ["dotnet", "Platform.Collector.Facebook.dll"]
