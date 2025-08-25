FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ContratacaoService.Api/*.csproj ./ContratacaoService.Api/
COPY ContratacaoService.Application/*.csproj ./ContratacaoService.Application/
COPY ContratacaoService.CrossCutting.IoC/*.csproj ./ContratacaoService.CrossCutting.IoC/
COPY ContratacaoService.Domain/*.csproj ./ContratacaoService.Domain/
COPY ContratacaoService.Infrastructure/*.csproj ./ContratacaoService.Infrastructure/
RUN dotnet restore "./ContratacaoService.Api/ContratacaoService.Api.csproj"

COPY . .
WORKDIR "/src/ContratacaoService.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.Api.dll"]