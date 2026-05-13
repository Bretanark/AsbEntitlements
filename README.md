# AsbEntitlements

AsbEntitlements is a small .NET 10 technical test implementation of a graph-backed entitlement check service.

The service answers a single question for client applications: can a subject perform a named permission on a resource? It models the access path in Neo4j as:

```text
Party -> PartyRole -> Entitlement -> Permission -> Resource
```

The check endpoint accepts a subject id, permission name, and resource id, then returns allow/deny, a reason, and the granted permission when access is allowed.

## Technical Test Context

The mandate was to design and implement the core of a centralised entitlement service for a financial organisation. Identities are customers represented by unique ids. Access is granted through Party Roles and Entitlements, with the model intended to align with BIAN-style concepts.

The requested scope was:

- Use a graph database, preferably Neo4j, to model identity -> role -> permission -> resource traversal.
- Expose a REST endpoint to evaluate an entitlement check.
- Include seed data that demonstrates the model.
- Write tests covering granted and denied checks.
- Do not use a pre-built authorisation library.

## Current Implementation

- API: .NET 10 minimal API
- Graph database: Neo4j
- Data access: `Neo4j.Driver`
- Tests: xUnit, `WebApplicationFactory`, and `Testcontainers.Neo4j`
- Demo seed data: loaded automatically on startup in Development only

The main endpoint is:

```http
POST /entitlements/check
```

Request:

```json
{
  "subjectId": "cust-001",
  "permissionName": "Account.Transfer",
  "resourceId": "acct-123"
}
```

Allowed response:

```json
{
  "allowed": true,
  "reason": "Permission 'Account.Transfer' granted via role 'AccountOwner'.",
  "grantedPermission": "Account.Transfer"
}
```

Denied response:

```json
{
  "allowed": false,
  "reason": "No matching entitlement found.",
  "grantedPermission": null
}
```

## Demo Data

In Development, the API clears and reseeds demo nodes marked with `demoSeed = true`.

The seeded graph is:

```text
cust-001
  HOLDS_ROLE -> AccountOwner
    HAS_ENTITLEMENT -> AccountOperations
      GRANTS_PERMISSION -> Account.Transfer
        APPLIES_TO -> acct-123
      GRANTS_PERMISSION -> Account.View
        APPLIES_TO -> acct-123

cust-002
  HOLDS_ROLE -> AccountViewer
    HAS_ENTITLEMENT -> ReadOnlyAccess
      GRANTS_PERMISSION -> Account.View
        APPLIES_TO -> acct-456
```

## New Developer Setup

Install:

- .NET 10 SDK
- Docker Desktop or another Docker runtime

Start Neo4j:

```powershell
docker compose up -d neo4j
```

Neo4j browser is available at:

```text
http://localhost:7474
```

Local credentials are configured in `Entitlements.Api/appsettings.Development.json`:

```text
Username: neo4j
Password: password
Bolt URI: bolt://localhost:7687
```

Restore and build:

```powershell
dotnet restore AsbEntitlements.sln
dotnet build AsbEntitlements.sln
```

Run the API:

```powershell
dotnet run --project Entitlements.Api
```

The default local HTTP URL is:

```text
http://localhost:5198
```

Swagger UI is available in Development at:

```text
http://localhost:5198/swagger
```

The generated OpenAPI document is available at:

```text
http://localhost:5198/openapi/v1.json
```

## Example Request

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri http://localhost:5198/entitlements/check `
  -ContentType 'application/json' `
  -Body '{"subjectId":"cust-001","permissionName":"Account.Transfer","resourceId":"acct-123"}'
```

## Running Tests

The integration tests start a real Neo4j container with Testcontainers and test the HTTP endpoint.

Docker must be running:

```powershell
dotnet test Entitlements.Tests/Entitlements.Tests.csproj
```

If the API is already running locally and locking the normal Debug output, use a separate test output folder:

```powershell
dotnet test Entitlements.Tests/Entitlements.Tests.csproj -o .verify-tests
```

## Project Notes

This implementation intentionally avoids CQRS, MediatR, generic repositories, authentication middleware, and pre-built authorisation libraries. The point of the exercise is the entitlement graph model and traversal, not framework wiring.

I used ChatGPT while building this because I was unfamiliar with Neo4j and the entitlement domain. The generated guidance was used as a development aid, with the code, tests, and README intended to make the final design understandable and runnable.

See [CONTRIBUTING.md](CONTRIBUTING.md) for coding standards and local contribution notes.
