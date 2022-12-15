# note: this needs to be pinned to an amd64 image in order to publish armv7 binaries
# https://github.com/dotnet/dotnet-docker/issues/1537#issuecomment-615269150
FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine-amd64 AS build-env
WORKDIR /app
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore src/MamisSolidarias.WebAPI.Campaigns/MamisSolidarias.WebAPI.Campaigns.csproj
# Build and publish a release
RUN dotnet publish src/MamisSolidarias.WebAPI.Campaigns/MamisSolidarias.WebAPI.Campaigns.csproj  \
    -c Release \
    -o out \
    --no-restore 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine

ENV OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT=4094

EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet","MamisSolidarias.WebAPI.Campaigns.dll"]
