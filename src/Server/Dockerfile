#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ENV ASPNETCORE_URLS=https://+:5005;http://+:5006
WORKDIR /home/edo
EXPOSE 5005
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
SHELL ["/bin/bash", "-c"]
COPY ["src/Server/Server.csproj", "src/Server/"]
COPY ["src/Server/Files", ""]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Infrastructure.Shared/Infrastructure.Shared.csproj", "src/Infrastructure.Shared/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Client/Client.csproj", "src/Client/"]
COPY ["src/Client.Infrastructure/Client.Infrastructure.csproj", "src/Client.Infrastructure/"]
RUN dotnet restore "src/Server/Server.csproj" --disable-parallel
COPY . .
WORKDIR "src/Server"
RUN dotnet build "Server.csproj" -c Release -o /home/edo/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /home/edo/publish

FROM base AS final
WORKDIR /home/edo
COPY --from=publish /home/edo/publish .
WORKDIR /home/edo/Files
WORKDIR /home/edo/Files/Manuals
WORKDIR /home/edo
COPY ["*.pdf", "/home/edo/Files/Manuals/"]
ENTRYPOINT ["dotnet", "EDO_FOMS.Server.dll"]
