# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

# Detect the project automatically when exactly one .csproj exists.
RUN set -eux; \
    project_count="$(find . -name '*.csproj' | wc -l)"; \
    if [ "$project_count" -ne 1 ]; then \
      echo "Expected exactly one .csproj file, found $project_count."; \
      exit 1; \
    fi; \
    project_path="$(find . -name '*.csproj' | head -n 1)"; \
    dotnet restore "$project_path"; \
    dotnet build "$project_path" -c Release --no-restore; \
    dotnet publish "$project_path" -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MamiaSeedsOil.Web.dll"]
