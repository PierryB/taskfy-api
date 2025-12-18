FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["pomodoro-api.csproj", "./"]
RUN dotnet restore "pomodoro-api.csproj"
COPY . .
RUN dotnet build "pomodoro-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pomodoro-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "pomodoro-api.dll"]
