FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY TestCore/TestCore.csproj TestCore/
RUN dotnet restore TestCore/TestCore.csproj
COPY . .
WORKDIR /src/TestCore
RUN dotnet build TestCore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TestCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TestCore.dll"]
