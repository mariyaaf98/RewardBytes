FROM mcr.microsoft.com/dotnet/aspnet:10.0-nanoserver-ltsc2022 AS base
WORKDIR /app
EXPOSE 5147

ENV ASPNETCORE_URLS=http://+:5147

FROM mcr.microsoft.com/dotnet/sdk:10.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["BytesRewards/src/BytesRewards.Service/BytesRewards.Service.csproj", "BytesRewards/src/BytesRewards.Service/"]
RUN dotnet restore "BytesRewards\src\BytesRewards.Service\BytesRewards.Service.csproj"
COPY . .
WORKDIR "/src/BytesRewards/src/BytesRewards.Service"
RUN dotnet build "BytesRewards.Service.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "BytesRewards.Service.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BytesRewards.Service.dll"]
