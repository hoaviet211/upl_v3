FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Academy.Crm.sln", "/src/"]
COPY ["Academy.Crm.Api/Academy.Crm.Api.csproj", "Academy.Crm.Api/"]
COPY ["Academy.Crm.Core/Academy.Crm.Core.csproj", "Academy.Crm.Core/"]
COPY ["Academy.Crm.Infra/Academy.Crm.Infra.csproj", "Academy.Crm.Infra/"]
RUN dotnet restore "Academy.Crm.Api/Academy.Crm.Api.csproj"
COPY . .
RUN dotnet publish "Academy.Crm.Api/Academy.Crm.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Academy.Crm.Api.dll"]

