Feature Flags Backend (ASP.NET Core 8)

Overview
--------
This is a minimal, interview-ready backend for a Feature Flag Management system.
It provides CRUD for feature flags, toggle, and evaluation APIs. A simple audit log records toggle/update actions.

Architecture
------------
- Single Web API project using a small Clean-ish separation: Models, DTOs, Services, Data, Controllers.
- `FlagService` contains business logic and a small in-memory cache for evaluation speed.
- EF Core + SQLite used for persistence (file-based DB for local/demo).

Evaluation logic (critical)
---------------------------
// Implemented in `FlagService.EvaluateAsync`
1) If flag is disabled -> return false (reason: "disabled").
2) If userId appears in `TargetUserIds` -> return true (reason: "targeted").
3) Else compute deterministic hash: for each character in userId, hash = (hash*31 + char) % 100.
   If hash < rolloutPercentage -> true (reason: "rollout_match").
4) Otherwise -> false (reason: "not_in_rollout").

Fail-safe: any exception or missing flag results in enabled=false.

Running locally
----------------
- No external DB required by default. The project uses SQLite file `FeatureFlags.db` in the backend folder.
- To change connection, edit `backend/appsettings.json`.
- From `backend` run:
  dotnet restore
  dotnet run --project FeatureFlags.Api.csproj

Notes / Tradeoffs
-----------------
- Targeted users are stored as CSV string (simple, realistic for small demo). In real SaaS you'd use proper targeting lists and rules.
- No authentication in this demo (explicit requirement).
- Cache is simple in-process ConcurrentDictionary; in production use a distributed cache.
- Migrations are not included; for serious use add EF migrations.
