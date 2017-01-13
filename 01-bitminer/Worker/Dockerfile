FROM microsoft/dotnet:1.1.0-sdk-projectjson

WORKDIR /dotnetapp

# copy project.json and restore as distinct layers
ADD project.json .
RUN dotnet restore

# copy and build everything else
ADD . .
ENTRYPOINT ["dotnet", "run", "-c", "Release"]