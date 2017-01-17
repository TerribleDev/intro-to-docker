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

