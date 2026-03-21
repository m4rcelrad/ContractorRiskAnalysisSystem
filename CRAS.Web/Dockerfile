FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CRAS.Web/CRAS.Web.csproj", "CRAS.Web/"]
COPY ["CRAS.Application/CRAS.Application.csproj", "CRAS.Application/"]
COPY ["CRAS.Domain/CRAS.Domain.csproj", "CRAS.Domain/"]
RUN dotnet restore "CRAS.Web/CRAS.Web.csproj"

COPY . .
WORKDIR "/src/CRAS.Web"
RUN dotnet publish "CRAS.Web.csproj" -c Release -o /app/publish

FROM nginx:alpine
WORKDIR /usr/share/nginx/html
COPY --from=build /app/publish/wwwroot .
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
