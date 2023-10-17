FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Workers/Workers.csproj", "Workers/"]
RUN dotnet restore "Workers/Workers.csproj"
COPY . .
WORKDIR "/src/Workers"
RUN dotnet build "Workers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Workers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Workers.dll"]
