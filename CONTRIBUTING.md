# Contributing

Thanks for helping improve AsbEntitlements.

## Development Standards

- Keep changes small, focused, and easy to review.
- Prefer simple working solutions over complex abstractions.
- Avoid broad refactors unless they are necessary for the change.
- Explain behavior changes clearly in commits, pull requests, or handoff notes.
- Keep API behavior explicit and add focused tests when changing logic or contracts.
- Prefer trailing commas in multiline lists and initializers where C# allows them.
- Do not commit generated files, local IDE state, build output, or secrets.

## Local Setup

Restore and build the solution:

```powershell
dotnet restore AsbEntitlements.sln
dotnet build AsbEntitlements.sln
```

Run the API:

```powershell
dotnet run --project Entitlements.Api
```

In Development, the OpenAPI document is available at:

```text
http://localhost:5198/openapi/v1.json
```

## Before Opening a Pull Request

- Run the relevant build and tests.
- Check that no local-only files are included.
- Keep the PR description focused on what changed and why.
