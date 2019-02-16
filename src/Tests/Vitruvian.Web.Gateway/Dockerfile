FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Vitruvian.Web.Gateway/Vitruvian.Web.Gateway.csproj", "Vitruvian.Web.Gateway/"]
RUN dotnet restore "Vitruvian.Web.Gateway/Vitruvian.Web.Gateway.csproj"
COPY . .
WORKDIR "/src/Vitruvian.Web.Gateway"
RUN dotnet build "Vitruvian.Web.Gateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vitruvian.Web.Gateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vitruvian.Web.Gateway.dll"]