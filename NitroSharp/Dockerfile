FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY bin/Relase/netcoreapp3.1/publish/ src/
WORKDIR /src
ENTRYPOINT dotnet NitroSharp.dll

VOLUME /Configs