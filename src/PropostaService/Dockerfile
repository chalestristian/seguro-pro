FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY PropostaService.Api/*.csproj ./PropostaService.Api/
COPY PropostaService.Application/*.csproj ./PropostaService.Application/
COPY PropostaService.CrossCutting.IoC/*.csproj ./PropostaService.CrossCutting.IoC/
COPY PropostaService.Domain/*.csproj ./PropostaService.Domain/
COPY PropostaService.Infrastructure/*.csproj ./PropostaService.Infrastructure/
RUN dotnet restore "./PropostaService.Api/PropostaService.Api.csproj"

COPY . .
WORKDIR "/src/PropostaService.Api"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PropostaService.Api.dll"]