FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CRAS.Api/CRAS.Api.csproj", "CRAS.Api/"]
COPY ["CRAS.Application/CRAS.Application.csproj", "CRAS.Application/"]
COPY ["CRAS.Domain/CRAS.Domain.csproj", "CRAS.Domain/"]
COPY ["CRAS.Infrastructure/CRAS.Infrastructure.csproj", "CRAS.Infrastructure/"]
RUN dotnet restore "CRAS.Api/CRAS.Api.csproj"

COPY . .
WORKDIR "/src/CRAS.Api"
RUN dotnet publish "CRAS.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CRAS.Api.dll"]
