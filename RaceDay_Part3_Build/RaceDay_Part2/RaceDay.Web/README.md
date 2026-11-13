# RaceDay MVC (Part 3)

ASP.NET Core MVC frontend for the RaceDay Part 2 REST API. The MVC project has no Entity Framework Core or direct SQL Server dependency; all business data is requested through the API.

## Requirements
- .NET 8 SDK
- RaceDay.API running and reachable

## Configure the API URL
Edit `appsettings.json` and set `Api:BaseUrl` to the HTTPS base address of your running API. Example: `https://localhost:7001/`.

## Run locally
From this directory:

```bash
dotnet restore
dotnet run
```

Open the URL printed by the ASP.NET Core host. Start the API first. For local development, trust the ASP.NET Core development certificate and use HTTPS because the Part 2 API authentication cookie is configured as Secure.

## Run with Docker
Build from this directory:

```bash
docker build -t raceday-web .
```

Run (replace the API URL with an address reachable from the container):

```bash
docker run --rm -p 8080:8080 -e Api__BaseUrl=http://host.docker.internal:7000/ raceday-web
```

The API must also be running. On Linux, `host.docker.internal` may need `--add-host=host.docker.internal:host-gateway`. For a Docker Compose deployment, put the MVC and API services on the same network and use the API service name as the base URL.

## Current implementation notes
- Registration, login, logout, session-backed user identity and role navigation are included.
- MVC data access is routed through `ApiClient` to the Part 2 API.
- Event browsing, organiser event CRUD, category creation, enrolment, profile fields and participant result views are scaffolded against the Part 2 endpoints.
- The current Part 2 API does **not yet** expose Azure Blob upload endpoints or image URL fields. Event banner and profile picture upload/display therefore remain to be implemented in the API and MVC before this POE requirement is complete.
- The Part 2 API uses `.RaceDay.Auth` with `SecurePolicy.Always`; use HTTPS for local end-to-end authentication. Review cookie forwarding and anti-forgery behaviour during integration testing.
- Run and verify the solution locally. This source package has not been compiled in this environment.
