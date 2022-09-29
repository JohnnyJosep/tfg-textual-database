#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/SpeechSearchSystem.Consumers/SpeechSearchSystem.Consumers.Freeling/SpeechSearchSystem.Consumers.Freeling.csproj", "src/SpeechSearchSystem.Consumers/SpeechSearchSystem.Consumers.Freeling/"]
RUN dotnet restore "src/SpeechSearchSystem.Consumers/SpeechSearchSystem.Consumers.Freeling/SpeechSearchSystem.Consumers.Freeling.csproj"
COPY . .
WORKDIR "/src/src/SpeechSearchSystem.Consumers/SpeechSearchSystem.Consumers.Freeling"
RUN dotnet build "SpeechSearchSystem.Consumers.Freeling.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpeechSearchSystem.Consumers.Freeling.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpeechSearchSystem.Consumers.Freeling.dll"]