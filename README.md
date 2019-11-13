<div align="center">
    <img src=".media/gh-banner-rendered.png">
    <br/>
    <a href="https://travis-ci.org/zekroTJA/vctr"><img src="https://travis-ci.org/zekroTJA/vctr.svg?branch=master"></a>
    <a href="https://hub.docker.com/r/zekro/vctr"><img src="https://img.shields.io/badge/docker-zekro%2Fvctr-16abc9?logo=docker&logoColor=16abc9"></a>
</div>

---

> This project is currently work in progress and not in a productive usable state. Keep track of following releases for actual usable versions.

vctr `[ˈvɛktər]` is a self hosted service to access, create, manage and analyze short links.

---

## Features / Roadmap

| Feature | Logic implemented | Implemented in API | Implemented in Front End |
| ------- | ----------------- | ------------------ | ------------------------ |
| Create short links that direct to another URL (example: `<host>/gh` → `https://github.com/zekroTJA`) | ✔️ | ✔️ | ✔️ |
| Enable/Disable shortlinks | ✔️ | ✔️ | ✔️ |
| Change short ident and/or root URL of short link | ✔️ | ✔️ | ✔️ |
| Set a access count limit for unique accesses | ✔️ | ✔️ | ✔️ |
| Chose between temporary *(uncached)* and permanent *(cached)* rediecting | ✔️ | ✔️ | ✔️ |
| Set an activation and/or expiration date for the short link | ✔️ | ✔️ | ✔️ |
| Configure server settings like password or default redirection | ✔️ | ✔️ | ✔️ |
| Analyse usage of short links | ✔️ | ✖️ | ❌ |

> - ✔️ fully implemented
> - ✖️ partially implemented but not fully functional yet
> - ❌ not implemented

## TO-DOs

- [ ] `[UI ]` General Settings View
- [ ] `[API]` Short link analysis details endpoint
- [ ] `[UI ]` Short link details and analysis

---

## Configuration

vctr uses following configuration structrue:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Debug"
        }
    },

    "ConnectionStrings": {
        "MySQL": "Server=localhost;Database=vctr;User Id=vctr;Password=password"
    },

    "Secrets": {
        "IPInfoToken": ""
    },
    
    "Server": {
        "URL": "localhost"
    }
}
```

Settings can be provided by:
- an `appsettings.json` file in the root direction
- environment variables  
  *Every key must start with `VCTR_` and groups are seperated by `__`, for example: `VCTR_CONNECTIONSTRINGS__MYSQL`*.
- command arguments  
  *Groups are seperated by `:`, for example: `--ConnectionStrings:MySQL`*.

---

## Setup

vctr currently requires a connection to a MySQL / MariaDB database to save persistent data.

### Docker

The recommendet way to set up and install vctr is by using the provided docker image, which can be built using the `Dockerfile` in this repository or by pulling the latest pre-built image from [Docker Hub](https://hub.docker.com/r/zekro/vctr) registry.

```
# docker pull zekro/vctr:latest
```

```
# docker run \
    --name vctr \
    --publish 80:80 \
    --env 'VCTR_CONNECTIONSTRINGS__MYSQL=Server=localhost;Database=vctr;User Id=vctr;Password=password' \
    --env 'VCTR_SERVER__URL=localhost' \
    --env 'VCTR_SECRETS__IPINFOTOKEN=token' \
    --detached \
    zekro/vctr:latest
```

Then, open up your browser and connect to the published address of `vctr` to initially set a password which is used  to access the service.

---

© 2019 Ringo Hoffmann (zekro Development).  
Covered by the MIT Licence.
