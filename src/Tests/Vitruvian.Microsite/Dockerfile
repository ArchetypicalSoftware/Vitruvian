FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

ENV "Vitruvian_Gateway_Url" "https://host.docker.internal:5001"

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Vitruvian.Microsite/Vitruvian.Microsite.csproj", "Vitruvian.Microsite/"]
RUN dotnet restore "Vitruvian.Microsite/Vitruvian.Microsite.csproj"
COPY . .
WORKDIR "/src/Vitruvian.Microsite"
RUN dotnet build "Vitruvian.Microsite.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vitruvian.Microsite.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vitruvian.Microsite.dll"]