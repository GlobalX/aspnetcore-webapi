# aspnetcore-webapi
ASP.NET 3.0 WEB API 
## Description

This repository is used an example to test multitenancy using postgres with Row Level Security and connecting to the database with Dapper

## Installation

- Install Docker
- Clone this branch locally
- Set a pwd for the postgres user in the below script and run it to start a docker postgres container:

```bash
docker run --rm --name pg-docker -v /your/temp/dir:/tmp -v /your/db/data/dir:/var/lib/postgresql/data -e POSTGRES_PASSWORD=<your-postgres-pwd> -d -p 5432:5432 postgres
```

- Install PG Admin (or run it within a container) and check that you can connect to the db server with the postgres user.

> the tmp and db/data directories in the above can be set to any local folder

## Usage

- Update the DBHost in appsettings.json to your local ip
- Update the AdminPassword in appsettings.json to your postgres user password
- Run the app in VS Code/VS/Rider
