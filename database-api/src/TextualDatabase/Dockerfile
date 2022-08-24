
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["src/TextualDatabase/TextualDatabase.csproj", "src/TextualDatabase/"]
COPY ["src/TextualDatabase.Application/TextualDatabase.Application.csproj", "src/TextualDatabase.Application/"]
COPY ["src/TextualDatabase.Domain/TextualDatabase.Domain.csproj", "src/TextualDatabase.Domain/"]
COPY ["src/TextualDatabase.Infrastructure/TextualDatabase.Infrastructure.csproj", "src/TextualDatabase.Infrastructure/"]

RUN dotnet restore "src/TextualDatabase/TextualDatabase.csproj"

COPY . .
WORKDIR "/src/src/TextualDatabase"
RUN dotnet build "TextualDatabase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TextualDatabase.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TextualDatabase.dll"]