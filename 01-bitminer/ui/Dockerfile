FROM node:boron-wheezy
RUN npm install -g bower
ADD package.json .
ADD .bowerrc .
RUN npm install
ADD bower.json .
RUN bower install --allow-root
ADD . .
EXPOSE 80
CMD ["npm", "run", "start"]

