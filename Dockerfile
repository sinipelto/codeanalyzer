FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic

RUN apt-get update; apt-get -y install maven

RUN mkdir -p /opt/analyser

COPY ./CodeAnalyzer /opt/analyser/CodeAnalyzer

COPY repositories.yml /opt/analyser/CodeAnalyzer/CodeAnalyzer.ConsoleApplication/Data/repositories.yml

RUN cd /opt/analyser/CodeAnalyzer; dotnet build -c Release

WORKDIR /opt/analyser/CodeAnalyzer/CodeAnalyzer.ConsoleApplication

CMD dotnet run --project CodeAnalyzer.ConsoleApplication.csproj -- sonar
