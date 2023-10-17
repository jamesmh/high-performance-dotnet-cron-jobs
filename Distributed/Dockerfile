FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Distributed/Distributed.csproj", "Distributed/"]
RUN dotnet restore "Distributed/Distributed.csproj"
COPY . .
WORKDIR "/src/Distributed"
RUN dotnet build "Distributed.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Distributed.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Distributed.dll"]
