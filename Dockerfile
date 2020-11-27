# This dockerfile is used for automated deployment of the Shatter bot and is not inteded for personal use.
# Please create your own Dockerfile to use Docker with this program.
FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY build/Shatter App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Shatter.dll"]