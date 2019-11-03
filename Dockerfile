FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY slms2asp.csproj slms2asp/
RUN dotnet restore "slms2asp/slms2asp.csproj"
COPY . .
WORKDIR /src/slms2asp
RUN dotnet build "slms2asp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "slms2asp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "slms2asp.dll"]