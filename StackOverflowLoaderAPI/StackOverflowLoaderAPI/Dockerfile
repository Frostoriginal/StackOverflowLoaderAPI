#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV ASPNETCORE_URLS http://*:7142
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /src
COPY StackOverflowLoaderAPI.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
ENV ASPNETCORE_URLS http://*:7142
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
#EXPOSE 80
#EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "StackOverflowLoaderAPI.dll"]


