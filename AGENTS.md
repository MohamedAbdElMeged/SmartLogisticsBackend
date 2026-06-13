# AGENTS — SmartLogisticsBackend

Purpose: give an AI coding agent the minimal, concrete knowledge needed to be immediately productive in this repository.

Quick checklist (what an agent will typically do first)
- Start the app + database locally
- Inspect the HTTP surface (Minimal API endpoints)
- Run/inspect database migrations
- Locate integration touchpoints (email, JWT, background jobs)

How to run (developer workflows)
- Local with Docker (recommended for first-time setup):
  1. From repo root: docker compose -f docker-compose-dev.yml up --build
     - This builds the `smartlogisticsbackend` dev image and starts `SmartLogisticsDB`.
     - The container runs `init.sh` which executes `dotnet-ef database update` and then `dotnet watch run`.
  2. Backend listens on port 5001 (see `docker-compose-dev.yml` and `SmartLogisticsBackend/Dockerfile.dev`).

- Run only the DB (fast iterative runs):
  - docker compose -f docker-compose-dev.yml up SmartLogisticsDB
  - then run the backend locally from the `SmartLogisticsBackend` folder:
    ```bash
    cd SmartLogisticsBackend
    dotnet restore
    dotnet tool install --global dotnet-ef || true
    dotnet ef database update
    dotnet watch run
    ```

- Quick dotnet run (if Postgres is already available):
  - cd `SmartLogisticsBackend` && dotnet run

Key architecture & patterns (what to look at)
- Minimal API + handler pattern: endpoints are defined as extension methods that call handler classes.
  - Example: `Features/Users/RegisterUser/Endpoint.cs` maps POST `/users/register` and calls `RegisterUserHandler`.
  - Handlers are registered as scoped services in `Program.cs` (e.g. `builder.Services.AddScoped<RegisterUserHandler>()`).

- Result wrapper for service → HTTP mapping:
  - Business code returns `Result` / `Result<T>` (`Common/Result.cs`).
  - `Common/ResultExtensions.cs` maps those to proper HTTP responses (Conflict/NotFound/422/401 etc.).
  - Agents should return or convert service responses using this pattern rather than throwing HTTP-specific exceptions.

- Validation: FluentValidation is used and wired in `Program.cs` via `AddValidatorsFromAssembly`.
  - Validators live next to requests, e.g. `Features/Users/RegisterUser/Validator.cs`.

- Persistence and migrations:
  - Entity Framework Core + PostgreSQL. Db context: `Infrastructure/Persistence/ApplicationDbContext.cs`.
  - Migrations are present in `Infrastructure/Persistence/Migrations/` (apply with `dotnet-ef database update`).

- Background jobs and email:
  - Hangfire configured to use Postgres storage (see `Program.cs` AddHangfire/queues and `UseHangfireServer`).
  - Jobs are enqueued with `IBackgroundJobClient` (see `Features/Users/RegisterUser/Handler.cs`) and job classes (e.g. `Infrastructure/BackgroundJobs/EmailJob.cs`).
  - Email is sent via the `Resend` client; configuration lives in `appsettings.Development.json` under `Resend:ApiToken` and `AppSettings:BaseUrl`.

- Authentication: JWT service
  - `Infrastructure/Auth/JwtTokenService.cs` implements `Common/Abstractions/IJwtTokenService`.
  - Settings are in `Infrastructure/Auth/JwtSettings.cs` and bound from configuration (`JwtSettings` in `appsettings.Development.json`).

Project-specific conventions and gotchas
- Handlers use primary-constructor style classes (constructed via DI) and are registered in `Program.cs`.
- Minimal-API mapping convention: every feature folder typically contains `Endpoint.cs`, `Handler.cs`, `Request/Response` records and `Validator.cs`.
- Use `Result` objects instead of throwing for expected domain errors — the HTTP mapping relies on the `ErrorType` enum.
- Verification tokens: `Domain/Entities/User.cs` stores only the hashed token; the raw token is sent by email. When testing, use the generated token returned by the create flow before hashing.

Integration points (where to stub/mock when writing tests or working offline)
- PostgreSQL DB: connection string in `SmartLogisticsBackend/appsettings.Development.json` (DefaultConnection). For CI, run a Postgres container.
- Resend email service: wired via `ResendClient` and `IResend` — replace with a test double implementing `IResend` or mock `IEmailSender`.
- Hangfire: uses same Postgres as storage. To test jobs integrate `IBackgroundJobClient` mocks or run Hangfire server locally (it is started by `Program.cs`).

Useful files to open first
- `Program.cs` (wiring, auth, Hangfire, validators)
- `Common/Result.cs` and `Common/ResultExtensions.cs` (error handling → HTTP)
- `Infrastructure/Persistence/ApplicationDbContext.cs` and `Infrastructure/Persistence/Migrations/`
- `Features/Users/*` (Register, Verify, Resend, Login) — canonical examples of the app flow
- `Infrastructure/Email/EmailSender.cs` and `Infrastructure/BackgroundJobs/EmailJob.cs`

Notes for PRs / code changes
- When adding endpoints, follow the existing feature folder pattern and register new handlers in `Program.cs`.
- New domain errors should map to `Result<T>` with the correct `ResultErrorType` so existing HTTP mapping continues to work.

If something is missing
- Look for configuration in `appsettings.json` and `appsettings.Development.json` (development-only behavior is enabled in `Program.cs`).

References
- Files mentioned inline: `Program.cs`, `Common/Result.cs`, `Common/ResultExtensions.cs`, `Infrastructure/Persistence/ApplicationDbContext.cs`, `Infrastructure/BackgroundJobs/EmailJob.cs`, `Infrastructure/Email/EmailSender.cs`, `SmartLogisticsBackend/Dockerfile.dev`, `init.sh`, `docker-compose-dev.yml`.

