<div align="center">
    <img src=".media/gh-banner-rendered.png">
    <hr>
    <h1>~ vctr ~</h1>
    <strong>
        simple self hosted short link service
    </strong><br><br>
    <a href="https://dc.zekro.de"><img height="28" src="https://img.shields.io/discord/307084334198816769.svg?style=for-the-badge&logo=discord" /></a>&nbsp;
    <a href="https://github.com/zekroTJA/vctr/releases"><img height="28" src="https://img.shields.io/github/tag/zekroTJA/vctr.svg?style=for-the-badge"/></a>&nbsp;
    <a href="https://hub.docker.com/r/zekro/shinpuru"><img alt="Docker Cloud Automated build" src="https://img.shields.io/docker/cloud/automated/zekro/shinpuru.svg?color=cyan&logo=docker&logoColor=cyan&style=for-the-badge"></a>&nbsp;
    <img height="28" src="https://forthebadge.com/images/badges/built-with-grammas-recipe.svg">
</div>

---

vctr `[ˈvɛktər]` is a self hosted service to access, create and manage short links on your domain.

## Setup

It is recommendet to use the provided Docker images or Dockerfiles to set up vctr.

- [`Gateway/Dockerfile`](Gateway/Dockerfile): The main vctr gateway
- [`WebApp/Dockerfile`](WebApp/Dockerfile): The default web frontend for vctr gateway

You can use the provided [`docker-compose.yml`](docker-compose.yml) config to set up the whole vctr environment with `traefik` as reverse proxy, `redis` as cache and `postgresql` as database.

After startup, go to `yourdomain.com/ui/login` and enter your predefined root account credentials.

### Configuration

You can either use the provided [`appsettings.json`](Gateway/appsettings.json) to configure the vctr gateway or use the following environment variables:
```yml
environment:
  VCTR_LOGGING__LOGLEVEL__DEFAULT: "Information"
  VCTR_LOGGING__LOGLEVEL__MICROSOFT: "Warning"
  VCTR_LOGGING__LOGLEVEL__MICROSOFT.HOSTING.LIFETIME: "Information"

  # Use either one of these:
  VCTR_CONNECTIONSTRINGS__POSTGRES: "Host=postgres;Database=vctr;Username=root;  Password=strong_postgres_root_passwrod"
  VCTR_CONNECTIONSTRINGS__MYSQL: "Host=mysql;Database=vctr;Username=root;  Password=strong_mysql_root_passwrod"

  VCTR_PASSWORDHASHING_MEMORYPOOLKB: 131072
  VCTR_PASSWORDHASHING_ITERATIONS: 4

  # Set this to have persistent sessions
  # after application restart
  VCTR_SESSIONS_JWTSECRET: "my_strong_jwt_secret"
  # Enable this is you are testing without an HTTPS
  # connection to the gateway
  VCTR_SESSIONS_BYPASSSECURECOOKIES: false

  VCTR_INITIALIZATION__ROOTUSERNAME: "root"
  VCTR_INITIALIZATION__ROOTUSERPASSWORD: "strong_vctr_root_passwrod"

  VCTR_CACHING__DURATION__LINKS: "30.00:00:00"
  VCTR_CACHING__REDIS__SERVERS: "redis:6379"
  VCTR_CACHING__REDIS__DATABASE: 0

  VCTR_ROUTING__ROOT: "https://zekro.de"
  VCTR_ROUTING__NOTFOUND: "/ui/notfound"
  VCTR_ROUTING__PASSWORD: "/ui/password"
```

---

## Migration

If you are using vctr < `v1.0`, you can use the [`Migrator`](Migrator) script to migrate links from old vctr instances to new (>= `v1.0`) vctr versions.

It is provided with a Dockerfile, so you do not need to install python on your host system.

```
$ docker build -t vctr-migrator . -f ./Migrator/Dockerfile
$ docker run --name vctr-migrator \
    -e CREATOR_GUID="USER_GUID_TO_BIND_LINKS_TO" \
    -e CONNECTIONSTRING_MYSQL="host=...;user=...;password=...;database=..." \
    -e CONNECTIONSTRING_PSQL="host=...;user=...;password=...;database=..."
```

---

## Community Projects ❤

- [Lukaesebrot/vctr-go](https://github.com/Lukaesebrot/vctr-go) - A wrapper for the vctr API written in go.
- [Lukaesebrot/vctr-cli](https://github.com/Lukaesebrot/vctr-cli) - A CLI tool connecting to the vctr API.

---

© 2020 Ringo Hoffmann (zekro Development).  
Covered by the MIT Licence.
