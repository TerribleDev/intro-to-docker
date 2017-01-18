FROM microsoft/dotnet:1.1.0-sdk-projectjson

WORKDIR /app

ADD project.json .

RUN ["dotnet", "restore"]

ADD . .

RUN ["dotnet", "build", "-c", "Release"]

ENTRYPOINT ["dotnet", "run", "-c", "Release"]