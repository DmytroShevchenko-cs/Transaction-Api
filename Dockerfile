# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["TransactionApi.Web/TransactionApi.Web.csproj", "TransactionApi.Web/"]
COPY ["TransactionApi.Service/TransactionApi.Service.csproj", "TransactionApi.Service/"]
COPY ["TransactionApi.Model/TransactionApi.Model.csproj", "TransactionApi.Model/"]

# Restore dependencies
RUN dotnet restore "TransactionApi.Web/TransactionApi.Web.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/TransactionApi.Web"
RUN dotnet build "TransactionApi.Web.csproj" -c Release -o /app/build
RUN dotnet publish "TransactionApi.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TransactionApi.Web.dll"]
