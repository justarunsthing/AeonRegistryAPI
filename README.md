# Aeon Registry API

A small HTTP API for a fictional field catalog. Seven sites are seeded, from the Mid-Atlantic Ridge to Mount Kailash. Each site has two stories: a public narrative, and an internal one that anonymous callers never receive.

This is an ASP.NET Core minimal API on .NET 10. PostgreSQL stores the sites, artifacts, images, and catalog notes. Identity issues bearer tokens. Swagger is the UI.

## Public and private

`GET /api/sites/1` returns the public record for the Mid-Atlantic Ridge site: name, location, coordinates, description, and `PublicNarrative`.

`GET /api/private/sites/1` returns that same record plus `InternalNarrative`. That call requires a bearer token. The same split exists on the way in: creating, updating, and deleting a site is on `/api/private/sites`.

Artifact images work the same way. Anyone can fetch an image by id. Uploading one requires a signed-in user.

## Seeded catalog

On first launch the API migrates the database and loads:

- 7 sites from `Data/SeedData/sites.json`
- Artifacts and per-site catalog records from the other JSON files in that folder
- PNG images from `Data/SeedData/Images`, stored on the artifact as file bytes

Catalog records carry a status of Draft, Verified, or Archived, plus the user who submitted the record, an optional verifier, and notes. The HTTP surface today is sites, images, and accounts. The catalog rows are there for the data model and the seed.

## Accounts

These users are created on an empty database. They are local seed accounts.

| Email | Password | Role |
| --- | --- | --- |
| admin@aeon.org | Admin123! | Admin |
| archivist@aeon.org | Archivist123! | Archivist |
| researcher@aeon.org | Researcher123! | Researcher |
| viewer@aeon.org | Viewer123! | Viewer |

The built-in Identity routes for register, forgot-password, reset-password, and manage info are blocked and return 404. Sign-in and token refresh stay available. Registration, password reset, and profile management go through the custom routes below. Registration mail is printed to the console by `ConsoleEmailService`, including the link to set a password.

## Run it

You need the .NET 10 SDK and PostgreSQL.

From the repository root:

```bash
dotnet user-secrets set "ConnectionStrings:DbConnection" "Host=localhost;Port=5432;Database=AeonRegistry;Username=postgres;Password=YOUR_PASSWORD"
dotnet run --launch-profile https
```

Swagger opens at `https://localhost:7103/swagger`. The first run applies migrations and can take a while while the seed images are written into PostgreSQL.

## Try the split

Public, no token:

```bash
curl -k https://localhost:7103/api/sites/1
curl -k https://localhost:7103/api/Home/welcome
```

Sign in:

```bash
curl -k -X POST https://localhost:7103/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@aeon.org","password":"Admin123!"}'
```

The response includes `accessToken`. Send it on private calls:

```bash
curl -k https://localhost:7103/api/private/sites/1 \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

Calling `/api/private/sites/1` without a token returns 401.

## Routes

| Method | Path | Auth |
| --- | --- | --- |
| GET | `/api/Home/welcome` | Public |
| GET | `/api/sites`, `/api/sites/{id}` | Public |
| GET | `/api/private/sites`, `/api/private/sites/{id}` | Bearer |
| POST | `/api/private/sites` | Bearer |
| PUT | `/api/private/sites/{id}` | Bearer |
| DELETE | `/api/private/sites/{id}` | Bearer |
| GET | `/api/public/artifacts/images/{id}` | Public. Cached for one day |
| POST | `/api/private/artifacts/{artifactId}/images` | Bearer. Multipart file, optional `isPrimary` |
| POST | `/api/auth/login`, `/api/auth/refresh` | Public |
| POST | `/api/auth/register-admin` | Public custom registration |
| POST | `/api/auth/forgot-password`, `/api/auth/reset-password` | Public custom password flow |
| GET, PUT | `/api/auth/manage/profile` | Bearer |
| GET | `/api/auth/manage/users` | Bearer |

Unhandled exceptions on the site and image groups pass through `ExceptionHandlingFilter`.

## Stack

- .NET 10 minimal APIs
- ASP.NET Core Identity with roles and bearer tokens
- Entity Framework Core, Npgsql, PostgreSQL
- Swashbuckle
