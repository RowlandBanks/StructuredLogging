#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["SampleApp/SampleApp.csproj", "SampleApp/"]
COPY ["MicrosoftExtensions/MicrosoftExtensions.csproj", "MicrosoftExtensions/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "SampleApp/SampleApp.csproj"
COPY . .
WORKDIR "/src/SampleApp"
RUN dotnet build "SampleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Arbee.StructuredLogging.SampleApp.dll"]
