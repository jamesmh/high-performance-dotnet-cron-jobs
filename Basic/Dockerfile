FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Basic/Basic.csproj", "Basic/"]
RUN dotnet restore "Basic/Basic.csproj"
COPY . .
WORKDIR "/src/Basic"
RUN dotnet build "Basic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Basic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basic.dll"]
