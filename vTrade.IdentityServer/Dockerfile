FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/appsettings.json.template ./appsettings.json.template
COPY --from=build-env /app/dockerStart.sh ./dockerStart.sh
ENTRYPOINT ./dockerStart.sh