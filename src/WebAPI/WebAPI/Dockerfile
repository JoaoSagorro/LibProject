# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

WORKDIR /source

COPY . .

ARG TARGETARCH

RUN dotnet restore "src/WebAPI/WebAPI/WebAPI.csproj"

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "src/WebAPI/WebAPI/WebAPI.csproj" -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

# Install ICU (International Components for Unicode) for globalization support
RUN apk add --no-cache icu-libs

# Set environment variables to enable globalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LC_ALL=en_US.UTF-8
ENV LANG=en_US.UTF-8

COPY --from=build /app .

USER $APP_UID

ENTRYPOINT ["dotnet", "WebAPI.dll"]

