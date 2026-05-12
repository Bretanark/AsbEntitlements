# Contributing

Thanks for helping improve AsbEntitlements.

## Development Standards

- Keep changes small, focused, and easy to review.
- Prefer simple working solutions over complex abstractions.
- Avoid broad refactors unless they are necessary for the change.
- Explain behavior changes clearly in commits, pull requests, or handoff notes.
- Keep API behavior explicit and add focused tests when changing logic or contracts.
- Prefer trailing commas in multiline lists and initializers where C# allows them.
- Generate and save text files with Windows line endings (CRLF), matching `.gitattributes` and `.editorconfig`.
- Keep shared ReSharper/Rider spelling and naming exceptions in `AsbEntitlements.sln.DotSettings`; do not commit `.DotSettings.user` files.
- Do not commit generated files, local IDE state, build output, or secrets.

## C# Style

- Keep code ReSharper-green: no orange ReSharper marks and no compiler warnings.
- Keep line wrapping light. Only wrap when a line is over roughly 200 characters.
- When wrapping method calls, declarations, or parameter lists, use one extra indent and allow multiple parameters on the continued line.
- Do not use braces for single-line `if` or `else` branches unless the other branch is multi-line.
- Keep very trivial conditionals on one line, for example `if (condition) return;`.
- Always assign awaited results to a variable before using them, so they are easy to inspect in the debugger.
- Add parameterized constructors where they avoid named-parameter-heavy call sites.
- Prefer no blank lines between trivial single-line fields or properties.
- Use two blank lines between type declarations, except when grouping multiple one-line types.

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
