# This dockerfile is used for automated deployment of the Shatter bot and is not inteded for personal use.
# Please create your own Dockerfile to use Docker with this program.
# build: docker build -t shatter-image -f Dockerfile .
# run: 

FROM mcr.microsoft.com/dotnet/aspnet:5.0

RUN apt-get update
RUN apt-get -y install curl
RUN apt-get --assume-yes install libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get --assume-yes install libc6-dev libgdiplus

COPY Shatter/build/Shatter App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Shatter.dll"]