FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/TextualDatabase/TextualDatabase.csproj", "TextualDatabase/"]
RUN dotnet restore "src/TextualDatabase/TextualDatabase.csproj"
COPY . .
WORKDIR "/src/TextualDatabase"
RUN dotnet build "TextualDatabase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TextualDatabase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TextualDatabase.dll"]
