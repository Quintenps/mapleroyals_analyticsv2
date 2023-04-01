FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["MapleRoyalsPlayerCount.csproj", "./"]
RUN dotnet restore "MapleRoyalsPlayerCount.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MapleRoyalsPlayerCount.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MapleRoyalsPlayerCount.csproj" -c Release -o /app/publish \
--runtime alpine-x64 \
--self-contained true \
/p:PublishTrimmed=true \
/p:TrimMode=Link \
/p:PublishSingleFile=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./MapleRoyalsPlayerCount"]
