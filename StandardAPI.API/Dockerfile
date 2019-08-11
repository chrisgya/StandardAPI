FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["StandardAPI.API/StandardAPI.API.csproj", "StandardAPI.API/"]
RUN dotnet restore "StandardAPI.API/StandardAPI.API.csproj"
COPY . .
WORKDIR "/src/StandardAPI.API"
RUN dotnet build "StandardAPI.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StandardAPI.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StandardAPI.API.dll"]