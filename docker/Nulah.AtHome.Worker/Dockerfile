# https://devblogs.microsoft.com/dotnet/announcing-dotnet-chiseled-containers/
# build the backend
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS workerbuild
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# this will copy everything into this image layer I think?
# this means any secrets in appsettings files will also be included, but I certainly won't be hosting this image
# on a hub so I don't really care yet.
COPY ["/backend/Nulah.AtHome.Api/Nulah.AtHome.Worker/", "Nulah.AtHome.Worker/"]
WORKDIR "Nulah.AtHome.Worker/"
RUN dotnet build "Nulah.AtHome.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# publish the backend
FROM workerbuild AS workerpublish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nulah.AtHome.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy 
WORKDIR /app
# copy the published worker
COPY --from=workerpublish /app/publish .
COPY ["yt-dlp", "deps/"]
ENTRYPOINT ["dotnet", "Nulah.AtHome.Worker.dll"]