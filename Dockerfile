ARG VERSION=6.0-focal
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS build-env

WORKDIR /app

ADD *.sln .
ADD PlutoDAO.Gov.Application/*.csproj ./PlutoDAO.Gov.Application/
ADD PlutoDAO.Gov.Domain/*.csproj ./PlutoDAO.Gov.Domain/
ADD PlutoDAO.Gov.Infrastructure/*.csproj ./PlutoDAO.Gov.Infrastructure/
ADD PlutoDAO.Gov.Test/*.csproj ./PlutoDAO.Gov.Test/
ADD PlutoDAO.Gov.Test.Integration/*.csproj ./PlutoDAO.Gov.Test.Integration/
ADD PlutoDAO.Gov.WebApi/*.csproj ./PlutoDAO.Gov.WebApi/
RUN dotnet restore

ADD PlutoDAO.Gov.Application/. ./PlutoDAO.Gov.Application/
ADD PlutoDAO.Gov.Domain/. ./PlutoDAO.Gov.Domain/
ADD PlutoDAO.Gov.Infrastructure/. ./PlutoDAO.Gov.Infrastructure/
ADD PlutoDAO.Gov.WebApi/. ./PlutoDAO.Gov.WebApi/

WORKDIR /app/PlutoDAO.Gov.WebApi

RUN dotnet publish \
  -c Release \
  -o ./output

FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS runtime

RUN adduser \
  --disabled-password \
  --home /app \
  --gecos '' app \
  && chown -R app /app
USER app

WORKDIR /app

COPY --from=build-env /app/PlutoDAO.Gov.WebApi/output .
ENV DOTNET_RUNNING_IN_CONTAINER=true \
  ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "PlutoDAO.Gov.WebApi.dll"]
