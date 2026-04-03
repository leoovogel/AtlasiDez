FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AtlasiDez.slnx .
COPY AtlasiDez.Api/AtlasiDez.Api.csproj AtlasiDez.Api/
COPY AtlasiDez.Application/AtlasiDez.Application.csproj AtlasiDez.Application/
COPY AtlasiDez.Domain/AtlasiDez.Domain.csproj AtlasiDez.Domain/
COPY AtlasiDez.Infrastructure/AtlasiDez.Infrastructure.csproj AtlasiDez.Infrastructure/
RUN dotnet restore AtlasiDez.Api/AtlasiDez.Api.csproj

COPY . .
RUN dotnet publish AtlasiDez.Api/AtlasiDez.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AtlasiDez.Api.dll"]
