FROM microsoft/aspnetcore-build:1.1

WORKDIR /app

ADD project.json .

RUN ["dotnet", "restore"]

ADD . .

RUN ["dotnet", "build", "-c", "Release"]

ENTRYPOINT ["dotnet", "run", "-c", "Release"]