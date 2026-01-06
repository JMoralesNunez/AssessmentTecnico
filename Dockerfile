# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files for restore
COPY ["CoursePlatform.sln", "./"]
COPY ["CoursePlatform.API/CoursePlatform.API.csproj", "CoursePlatform.API/"]
COPY ["CoursePlatform.Application/CoursePlatform.Application.csproj", "CoursePlatform.Application/"]
COPY ["CoursePlatform.Domain/CoursePlatform.Domain.csproj", "CoursePlatform.Domain/"]
COPY ["CoursePlatform.Infrastructure/CoursePlatform.Infrastructure.csproj", "CoursePlatform.Infrastructure/"]
COPY ["CoursePlatform.Tests/CoursePlatform.Tests.csproj", "CoursePlatform.Tests/"]

# Restore dependencies
RUN dotnet restore

# Copy the remaining files and build
COPY . .
WORKDIR "/src/CoursePlatform.API"
RUN dotnet build "CoursePlatform.API.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "CoursePlatform.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "CoursePlatform.API.dll"]
