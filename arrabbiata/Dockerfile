ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY arrabbiata.csproj .
RUN dotnet restore arrabbiata.csproj

COPY . .
RUN dotnet publish arrabbiata.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS run
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/data

ENTRYPOINT ["dotnet", "arrabbiata.dll"]

