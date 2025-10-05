FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TransactionApi.Web/TransactionApi.Web.csproj", "TransactionApi.Web/"]
COPY ["TransactionApi.Service/TransactionApi.Service.csproj", "TransactionApi.Service/"]
COPY ["TransactionApi.Model/TransactionApi.Model.csproj", "TransactionApi.Model/"]
RUN dotnet restore "TransactionApi.Web/TransactionApi.Web.csproj"
COPY . .
WORKDIR "/src/TransactionApi.Web"
RUN dotnet build "TransactionApi.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TransactionApi.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TransactionApi.Web.dll"]
