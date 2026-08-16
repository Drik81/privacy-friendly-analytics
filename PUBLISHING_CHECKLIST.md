# Publishing checklist

- [ ] Create the GitHub repository as `privacy-friendly-analytics`.
- [ ] Confirm that no local secrets or real telemetry are present.
- [ ] Run `dotnet restore`.
- [ ] Run `dotnet build`.
- [ ] Run `dotnet test`.
- [ ] Start SQL Server with `docker compose up -d`.
- [ ] Apply the EF Core migration.
- [ ] Run the demo and verify ingestion and dashboard metrics.
- [ ] Capture one clean dashboard screenshot and add it under `docs/images/`.
- [ ] Add the screenshot to the README.
- [ ] Add the repository description: `Self-hosted, privacy-friendly product analytics reference implementation for ASP.NET Core.`
- [ ] Add GitHub topics: `dotnet`, `aspnet-core`, `ef-core`, `sql-server`, `typescript`, `analytics`, `privacy`, `self-hosted`.
- [ ] Enable Issues.
- [ ] Add the repository URL to the related DrikWeb article and portfolio entry.
