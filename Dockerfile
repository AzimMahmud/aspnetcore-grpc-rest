# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY aspnetapp/*.csproj ./aspnetapp/
RUN dotnet restore

# copy everything else and build app
COPY aspnetapp/. ./aspnetapp/
WORKDIR /app/aspnetapp
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./

# install libmsquic dependencies
RUN apt update \
    && apt install -y --no-install-recommends \
        curl \
        wget \
        gnupg2 \
        software-properties-common

RUN curl -O https://packages.microsoft.com/debian/10/prod/pool/main/libm/libmsquic/libmsquic-1.9.0-amd64.deb
RUN dpkg -i libmsquic-1.9.0-amd64.deb

ENTRYPOINT ["dotnet", "aspnetapp.dll"]
