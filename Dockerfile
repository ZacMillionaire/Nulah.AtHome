# https://devblogs.microsoft.com/dotnet/announcing-dotnet-chiseled-containers/
# build the backend
FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS backendbuild
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# this will copy everything into this image layer I think?
# this means any secrets in appsettings files will also be included, but I certainly won't be hosting this image
# on a hub so I don't really care yet.
COPY ["/backend/Nulah.AtHome.Api/Nulah.AtHome.Api/", "Nulah.AtHome.Api/"]
COPY ["/backend/Nulah.AtHome.Api/Nulah.AtHome.Data/", "Nulah.AtHome.Data/"]
WORKDIR "Nulah.AtHome.Api/"
RUN dotnet build "Nulah.AtHome.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# build the frontend
FROM node as frontendbuild
COPY ["/frontend/", "frontend/"]
RUN mkdir -p /home/node/app/node_modules && chown -R node:node /home/node/app
WORKDIR /frontend
RUN npm install
RUN npm run build

# publish the backend
FROM backendbuild AS backendpublish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nulah.AtHome.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled 
# listen port will be set to 8080 from image environment variables and I don't really care to change it
EXPOSE 8080
WORKDIR /app
# copy the published backend
COPY --from=backendpublish /app/publish .
# copy the built frontend static files into the published wwwroot folder
COPY --from=frontendbuild /frontend/build ./wwwroot
# expose the wwwroot volume to hint to people that they can change the frontend with whatever they want
VOLUME /app/wwwroot
ENTRYPOINT ["dotnet", "Nulah.AtHome.Api.dll"]
