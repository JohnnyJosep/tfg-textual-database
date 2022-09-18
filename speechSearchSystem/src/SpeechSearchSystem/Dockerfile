#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/SpeechSearchSystem/SpeechSearchSystem.csproj", "src/SpeechSearchSystem/"]
COPY ["src/SpeechSearchSystem.Application/SpeechSearchSystem.Application.csproj", "src/SpeechSearchSystem.Application/"]
COPY ["src/SpeechSearchSystem.Domain/SpeechSearchSystem.Domain.csproj", "src/SpeechSearchSystem.Domain/"]
COPY ["src/SpeechSearchSystem.Infrastructure/SpeechSearchSystem.Infrastructure.csproj", "src/SpeechSearchSystem.Infrastructure/"]
RUN dotnet restore "src/SpeechSearchSystem/SpeechSearchSystem.csproj"
COPY . .
WORKDIR "/src/src/SpeechSearchSystem"
RUN dotnet build "SpeechSearchSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpeechSearchSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpeechSearchSystem.dll"]