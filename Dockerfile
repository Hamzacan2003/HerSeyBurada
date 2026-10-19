FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["HerSeyBurada.WebMvcUI/HerSeyBurada.WebMvcUI.csproj", "HerSeyBurada.WebMvcUI/"]
COPY ["HerSeyBurada.Business/HerSeyBurada.Business.csproj", "HerSeyBurada.Business/"]
COPY ["HerSeyBurada.DataAccess/HerSeyBurada.DataAccess.csproj", "HerSeyBurada.DataAccess/"]
COPY ["HerSeyBurada.Core/HerSeyBurada.Core.csproj", "HerSeyBurada.Core/"]

RUN dotnet restore "HerSeyBurada.WebMvcUI/HerSeyBurada.WebMvcUI.csproj"

COPY . .
WORKDIR "/src/HerSeyBurada.WebMvcUI"
RUN dotnet publish "HerSeyBurada.WebMvcUI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Eksik olan Kerberos / GSSAPI kütüphanelerini Linux sunucusuna kuruyoruz:
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgssapi-krb5-2 \
    krb5-user \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HerSeyBurada.WebMvcUI.dll"]