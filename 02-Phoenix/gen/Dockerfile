FROM microsoft/dotnet:1.1.0-sdk-projectjson

WORKDIR /app

ADD . .

RUN ["dotnet", "restore"]

RUN ["dotnet", "build", "-c", "Release"]

EXPOSE 80/tcp

CMD ["dotnet", "run", "-c", "Release", "--server.urls", "http://*:80"]