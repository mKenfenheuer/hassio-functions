#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS buildui
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build
WORKDIR "/src/HAFunctions.UI"
RUN dotnet build "HAFunctions.UI.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS buildhost
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build
WORKDIR "/src/HAFunctions.FunctionsHost"
RUN dotnet build "HAFunctions.FunctionsHost.csproj" -c Release -o /app/build

FROM buildui AS publishui
RUN dotnet publish "HAFunctions.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM buildhost AS publishhost
RUN dotnet publish "HAFunctions.FunctionsHost.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publishhost /app/publish/ .
COPY --from=publishui /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "HAFunctions.UI.dll"]
