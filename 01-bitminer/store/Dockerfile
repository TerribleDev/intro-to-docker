FROM ruby:alpine
RUN apk add --update build-base
RUN gem install sinatra
RUN gem install thin
ADD app.rb /
CMD ["ruby", "app.rb"]
EXPOSE 80