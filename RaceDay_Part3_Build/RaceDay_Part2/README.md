# RaceDay – Parts 1, 2 and 3

RaceDay is an event-management platform for running, walking and cycling events. The solution contains the Part 2 REST API, its test project, the Part 1 planning artefacts, and the Part 3 ASP.NET Core MVC frontend.

## Solution projects
- `RaceDay.API` – ASP.NET Core Web API, Entity Framework Core and SQL Server access.
- `RaceDay.Tests` – xUnit tests for selected authentication and enrolment logic.
- `RaceDay.Web` – ASP.NET Core MVC frontend. It consumes the API through `ApiClient` and does not connect directly to SQL Server.
- `RaceDay_Part1` – original planning documentation and database design.

## Requirements
.NET 8 SDK and SQL Server. Azure Blob Storage is required for image upload functionality.

## Configure and run the API
1. Set `ConnectionStrings:RaceDay` in `RaceDay.API/appsettings.json` or user secrets.
2. Set `ConnectionStrings:AzureBlobStorage` to a valid Azure Storage connection string. Do not commit real credentials.
3. Create and apply an EF Core migration after the new `BannerImageUrl` and `ProfilePictureUrl` properties were added:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add AddRaceDayImageUrls --project RaceDay.API
dotnet ef database update --project RaceDay.API
dotnet run --project RaceDay.API
```

The API uses `.RaceDay.Auth` as its authentication cookie. The MVC application forwards that API cookie server-side while keeping its own role and user details in ASP.NET Core session.

## Run MVC
Set `Api:BaseUrl` in `RaceDay.Web/appsettings.json` to the API base URL, then:

```bash
dotnet run --project RaceDay.Web
```

## Docker MVC application
Build:

```bash
docker build -t raceday-web ./RaceDay.Web
```

Run (replace the API address with one reachable from the container):

```bash
docker run --rm -p 8080:8080 -e Api__BaseUrl=http://host.docker.internal:7000/ raceday-web
```

On Linux, add `--add-host=host.docker.internal:host-gateway` if needed. The API must be running separately. For production, use HTTPS between services and provide secrets through a secure configuration provider.

## CI
GitHub Actions restores and builds the full solution and runs the test project on pushes and pull requests to `main`, `master`, or `part-3`.

## Important verification status
This is a development scaffold based on the supplied Part 2 source. It has not been compiled or executed in this environment. Run the solution locally, resolve any environment-specific issues, verify API and MVC cookie/session behaviour, test all role-specific flows, and confirm the GitHub Actions workflow passes before submission. The Part 3 UI covers the core browsing, account, organiser and participant flows; image upload requires a configured Azure account and the EF Core migration above. Review and adapt the implementation and disclose AI assistance according to the assessment instructions.
