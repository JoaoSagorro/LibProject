# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /source

# Copy the entire solution
COPY . .

ARG TARGETARCH

# Restore the solution
RUN dotnet restore "src/WebAPI/WebAPI/WebAPI.csproj"

# Build and publish the application
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "src/WebAPI/WebAPI/WebAPI.csproj" -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "WebAPI.dll"]

