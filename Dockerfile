FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY ./ ./mysoundauth/
RUN dotnet restore ./mysoundauth/MySound.Auth.Api/MySound.Auth.csproj

# copy everything else and build app
COPY / ./mysoundauth/
WORKDIR /app/mysoundauth/MySound.Auth.Api
RUN dotnet publish -c Release -o ../out


FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/mysoundauth/out ./
ENTRYPOINT ["dotnet", "MySound.Auth.dll"]