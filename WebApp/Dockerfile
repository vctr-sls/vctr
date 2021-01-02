# --- WEBAPP BUILD STAGE

FROM node:15-alpine as build-web
ARG PUBLIC_URL="/ui"
WORKDIR /build
COPY ./WebApp/ .
RUN apk add --no-cache --virtual .gyp python make g++ python2 py2-pip
ENV PUBLIC_URL=$PUBLIC_URL
RUN npm ci &&\
    npm run build

# --- SEITEKI BUILD STAGE

FROM golang:1.15-alpine AS build-stk
WORKDIR /build
RUN apk add git
RUN git clone https://github.com/zekroTJA/seiteki . --depth 1
RUN go build -o seiteki cmd/seiteki/main.go

# --- FINAL STAGE

FROM alpine:latest AS final
LABEL maintainer="zekro <contact@zekro.de>"
WORKDIR /app
COPY --from=build-web /build/build/ .
COPY --from=build-stk /build/seiteki /bin/seiteki
RUN chmod +x /bin/seiteki
ENTRYPOINT ["/bin/seiteki", "-dir", "/app"]
CMD ["-addr", ":80"]
