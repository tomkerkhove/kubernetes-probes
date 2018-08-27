FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY TomKerkhove.Samples.Service/TomKerkhove.Samples.Service.csproj TomKerkhove.Samples.Service/
COPY TomKerkhove.Probes/TomKerkhove.Probes.csproj TomKerkhove.Probes/
RUN dotnet restore TomKerkhove.Samples.Service/TomKerkhove.Samples.Service.csproj
COPY . .
WORKDIR /src/TomKerkhove.Samples.Service
RUN dotnet build TomKerkhove.Samples.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TomKerkhove.Samples.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TomKerkhove.Samples.Service.dll"]
