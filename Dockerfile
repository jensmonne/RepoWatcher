FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Repowatcher/Repowatcher.csproj Repowatcher/
RUN dotnet restore

COPY . .
RUN dotnet publish Repowatcher/Repowatcher.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:5000

COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Repowatcher.dll"]
