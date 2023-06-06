# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ClimbTimerService/*.csproj ./ClimbTimerService/
RUN dotnet restore

# copy everything else and build app
COPY ClimbTimerService/. ./ClimbTimerService/
WORKDIR /source/ClimbTimerService
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

# Link repostitory
LABEL org.opencontainers.image.source=https://github.com/bordewich-software/ClimbTimerService
LABEL org.opencontainers.image.description="ClimbTimer Backend Service"

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ClimbTimerService.dll"]
