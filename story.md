
You are a (somewhat) compentant software enginer that has just joined a new company called bitminer. Bitminer is a fancy crypto currency mining platform, and you have been brought on because you said you knew something about docker in your resume. You are given a laptop, and told to show up the first day with docker, installed (see Readme.md](Readme.md) for specific setup instructions ). You know nothing about the current technology but have heard they are porting a legacy system to dotnet core.

## Day 01

You show up on your first day, and are told to `cd` into the bitminer folder (cd into `01-bitminer`), and they ask you to run `docker-compose up`.

As the text starts flying by, you notice a python app, a ruby app, a javascript app, and 2 dotnet apps being compiled. You assume these people know tons about microservices. You browse to `localhost:8000` and are met with a graph showing coins being mined on your machine. They ask you to stop the app <kbd>Ctrl</kbd> + <kbd>c</kcb> or <kbd>Cmd</kbd> + <kbd>c</kcb>

Your first task as a new employee is to find out why the company cannot mine dockercoins beyond a certain scale. They inform you that they have a fleet of `workers` that talk to a python service called `gen`, which serves up the current blockchain transaction. There is a `hashr` app that hashes the transaction, and a `store` for databasing. All of these individual services are being used by seperate `workers`. 

First you start the system in detached mode `docker-compose up -d`, and you pull up `localhost:8000` in your browser. You start scaling up the `workers` by running `docker-compose scale worker=5`. Everything seems normal, you see a standard uptick in coin mining. You then go for crazy mode and scale up the workers more `docker-compose scale worker=20` You definitely notice an increase in coin mining, but diminishing returns overall.

You start looking into `gen`, as you assume all python is terrible. On line 21 you notice a clear performance mistake. Someone has accidentally added a sleep to python! You delete the line (please delete line 21). You rebuild the docker image and boot it `docker-compose up -d --build gen`

You now being to mine coins much more efficiently! You are highly impressed by their use of `docker-compose` a tool that can fit many docker containers together (more details to come). You run `docker-compose down` to stop all of the docker containers

## Day 02 - The Phoenix project

Impressed by your skills you are put on the Phoenix project. Project Phoenix is, a re-write of the existing infrastructure, because as you all know rewrites are the best way to remove performance bottlenecks. During this rewrite they are unifying almost everything to dotnet, with the exception of the UI team, whom believe its just not cool if its not `js`. Furthermore because you are so knowledgable with docker, they didn't bother docker-izing anything. You are expected to do it without knowing anything about their services, since you are the expert.

You talk to Brent, bitminer's genius application developer. Brent asks you to start off by docker-izing `Hashr` service, noting that he already did the `gen` service.


 You cd into `02-Phoenix/Hashr`, and create a file called `Dockerfile`. You know this is the file docker will use to build the docker image. You read the Hashr `project.json` file, and realize its an dotnet core 1.1 project. You browse to [docker hub](https://hub.docker.com/r/microsoft/dotnet/) to find all docker containers with dotnet builtin. You come across image `1.1.0-sdk-projectjson`, which is what you are looking for. You notice there are a few other images. There are images that do not have an sdk installed, but just the runtime. these images are much smaller, but require you to compile your application outside the container and instead pass your dll's in. Since you don't have any build systems setup, you stick with the sdk, as its faster to get going on.

You add the following to your Dockerfile

```
FROM microsoft/aspnetcore-build:1.1

WORKDIR /app

```

You then need to pass in the project.json file and restore nuget packages

```
ADD project.json project.json

RUN ["dotnet", "restore"]

```

Next add all your source code and compile
```
ADD . .

RUN ["dotnet", "build", "-c", "Release"]
```

After you need to expose port 80 so the container can route http traffic to your app, and you have to give docker a command to run when the container starts. In this case it will be hashr

```
EXPOSE 80/tcp

CMD ["dotnet", "run", "-c", "Release", "--server.urls", "http://*:80"]
```

The full file should look like this:

```
FROM microsoft/aspnetcore-build:1.1

WORKDIR /app

ADD project.json project.json

RUN ["dotnet", "restore"]

ADD . .

RUN ["dotnet", "build", "-c", "Release"]

EXPOSE 80/tcp

CMD ["dotnet", "run", "-c", "Release", "--server.urls", "http://*:80"]

```

You run `docker build . -t hashr` which builds the container locally and tags it with the name hashr. You can see it listed if you run `docker images`. You can now run the container locally by executing `docker run -p 8080:80 -d -t hashr` and browsing to `localhost:8080` In the command `-p 8080:80`, you are mapping your local port 8080 to the containers port 80. We can list all running containers by executing `docker ps`. We can stop our container by running `docker kill HashrShaId` or you can kill all running containers by executing `docker kill $(docker ps -q)`


Since the UI didn't change you figure you can just copy over the Dockerfile from `01-bitminer/ui`. You read it


```js

FROM node:boron-wheezy
RUN npm install -g bower
ADD package.json .
RUN npm install
ADD .bowerrc .
ADD bower.json .
RUN bower install --allow-root
ADD . .
EXPOSE 80
CMD ["npm", "run", "start"]

```

From boron (which is node 6.9) on wheezy (a standard debian distro, node docker images can be [found here](https://hub.docker.com/_/node/)). Install bower, add the package.json file, npm install (install the npm modules required for your app), add the bower.json file and get the dependencies from bower (bower is a client side dependency manager). Then expose the right port, and run npm run start to boot the app when the container starts. This looks fine! Add this as a Dockerfile in `ui/`. 

You can also copy the Dockerfile from the worker `01-bitminer/worker`. This file is much like the hashr service.


You show your work to brent, "Excellent" he says. "We need a story to run the services locally, Can you tackle that". "I'm on it" you reply.


You know docker-compose is a great way to get started running these tools locally. You make a new docker-compose.yml file


```yml

version: "2"

services:
  gen:
    build: ./gen
    ports:
    - "8001:80"

  hashr:
    build: ./Hashr
    ports:
    - "8002:80"

  ui:
    build: ./ui
    ports:
    - "8000:80"
    links:
    - store

  store:
    build: ./store
    ports:
    - "8003:80"

  worker:
    build: ./worker
    links:
     - hashr
     - gen
     - store

```

This file declares services called ge, hashr, ui, store, and worker. it also shows where to find the Dockerfiles to build the containers, and any images that need to be "linked" or networked together. This will cause docker-compose to inject appropriate dns entries to dependent containers.

You run `docker-compose up -d`, and browse to `localhost:8000`. You see the coins being mined again. However you notice the performance bug with `gen` has been ported over. You remove line 22 of `02-Pheonix/gen/Controllers/ValuesController.cs`, and run `docker-compose up -d --build gen`. You notice the container boots, but for some reason the nuget packages were installed again. Since you only edited source code, and not the project.json file this should not occur.

You look at the Dockerfile for the `worker` service. You move line 5 to 8 (after dotnet restore) and you add `ADD project.json .` to line 5. This will only add the project.json file first before nuget restoring, which will tell docker to only nuget restore if that file has changed.

You `docker-compose down` and then `docker-compose up --build` one more time and everything works!

You have now dockerized your first set of services. Your career is just getting started at bitminer, and you never know what the future will bring!