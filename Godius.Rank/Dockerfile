FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Godius.Rank/Godius.RankSite.csproj", "Godius.Rank/"]
COPY ["Godius.Data/Godius.Data.csproj", "Godius.Data/"]
RUN dotnet restore "Godius.Rank/Godius.RankSite.csproj"
COPY . .
WORKDIR "/src/Godius.Rank"
RUN dotnet build "Godius.RankSite.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Godius.RankSite.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Godius.RankSite.dll"]