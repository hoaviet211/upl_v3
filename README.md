# UPL Academy CRM Light (NET 8)

A minimal CRM for UPL Academy following a clean 3‑layer architecture (Api/Core/Infra), built with .NET 8, EF Core (SQL Server), JWT, Swagger, and Serilog. It implements the provided OpenAPI, supports soft deletes, and offers pagination, sorting, free‑text search, and flexible filtering.

Swagger UI: http://localhost:8080/swagger

## Architecture
- Api: `Academy.Crm.Api` — ASP.NET Core Web API (Swagger, JWT, Serilog)
- Core: `Academy.Crm.Core` — Domain (Entities/Enums) + Interfaces (Repository/UnitOfWork)
- Infra: `Academy.Crm.Infra` — EF Core (SqlServer), DbContext, Fluent configurations, Repositories, Seed
- Tests: `Academy.Crm.Tests` — xUnit + EF Core Sqlite (in‑memory) acceptance tests

## Prerequisites
- .NET 8 SDK
- SQL Server 2022 (local or via Docker)
- EF Core tools (optional, recommended): `dotnet tool install --global dotnet-ef`

## Configuration
File: `Academy.Crm.Api/appsettings.json`
- `ConnectionStrings:Default` — SQL Server connection string
- `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` — JWT HS256 settings
- `Storage:BasePath` — relative folder for profile images (served as static files, e.g., `storage`)
- `FileUpload:MaxMB` — upload size limit in MB

Example (already included):
```
"ConnectionStrings": {
  "Default": "Server=localhost,1433;Database=AcademyCrm;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
},
"Jwt": {
  "Issuer": "UPL.Academy",
  "Audience": "UPL.Academy.Clients",
  "Key": "CHANGE_ME_SUPER_SECRET_DEVELOPMENT_KEY"
},
"Storage": { "BasePath": "storage" },
"FileUpload": { "MaxMB": 10 }
```

## Getting Started (Local)
1) Adjust `appsettings.json` if needed.
2) Create database and run:
```
dotnet ef database update
dotnet run --project Academy.Crm.Api
```
If you don’t have migrations yet, create them first:
```
dotnet ef migrations add InitialCreate -p Academy.Crm.Infra -s Academy.Crm.Api
dotnet ef database update -p Academy.Crm.Infra -s Academy.Crm.Api
```
Then open: http://localhost:8080/swagger

## Seeded Accounts and Data
- Admin: `admin` / `testing`
- Student: `thanhtam` / `testing`
- 1 Programme (NCLD)
- 2 Courses (NCLD-K71, NCLD-K72)
- 6 ClassSessions per Course

Seeding runs on startup (applies `Database.Migrate()` and inserts initial data if empty).

## Authentication (JWT)
- Login: `POST /auth/login`
Body:
```
{ "userName": "admin", "password": "testing" }
```
- Response: `{ accessToken, refreshToken }`
- Use: set header `Authorization: Bearer <accessToken>` for secured endpoints.

## Entities and Constraints (highlights)
- Programme(Id, ProgrammeName, AdmissionCycle)
- Course(Id, CourseCode, CourseName, ProgrammeId, StartDate, EndDate) — FK ProgrammeId, index ProgrammeId
- ClassSession(Id, CourseId, SessionCode, Title, ..., Mode, Status) — FK CourseId, index CourseId
- Student(Id, StudentCode UNIQUE, FirstName, LastName, PhoneNumber, ...)
- IdCard(Id, StudentId UNIQUE 1‑1, CardNumber, DateOfIssue, PlaceOfIssue)
- Enrollment(Id, StudentId, CourseId, Status, RegisteredAt, PaymentStatus, TuitionFee, Discount, Note)
  - UNIQUE(StudentId, CourseId); index StudentId, CourseId
- Attendance(Id, EnrollmentId, ClassSessionId, Status, Method, CheckInTime, Note)
  - UNIQUE(EnrollmentId, ClassSessionId); index EnrollmentId, ClassSessionId
- Soft delete: `IsDeleted` with a global query filter

## Query: Paging, Sorting, Search, Filter
- Paging: `page`, `pageSize` (capped at 100)
- Sorting: `sort` — comma‑separated, prefix with `-` for descending (e.g., `startDate,-courseCode`)
- Search: `q` — applied to a subset of text fields (per controller)
- Filters: `filter[Field][op]=value`
  - Operators: `eq`, `gte`, `lte`, `in`, `between`, `contains`
  - Examples:
    - `filter[ProgrammeId][eq]=1`
    - `filter[StartDate][between]=2025-01-01|2025-03-31`
    - `filter[Status][in]=Pending,Active`
    - `filter[CourseCode][contains]=K71`

## API Endpoints
- Auth
  - `POST /auth/login` → `{ accessToken, refreshToken }`
- Students (JWT)
  - `GET /students` (supports q, filter, sort, page)
  - `POST /students`
  - `GET /students/{id}`
  - `PUT /students/{id}`
  - `GET /students/me`
  - `POST /students/{id}/image` — upload profile image (returns relative `ImagePath`)
- Courses (JWT)
  - `GET /courses` (filters ProgrammeId/StartDate/EndDate; q/sort/page)
  - `POST /courses`
  - `GET /courses/{id}`
  - `PUT /courses/{id}`
- Enrollments (JWT)
  - `GET /enrollments` (filters StudentId/CourseId/Status/PaymentStatus)
  - `POST /enrollments` (admin create)
  - `POST /courses/{courseId}/enroll` (self‑enroll using current JWT student)
- Attendance (JWT)
  - `GET /attendance` (filters CourseId/StudentId/Date between)
  - `POST /attendance/checkin` (unique per (EnrollmentId, ClassSessionId))
- Reports (JWT)
  - `GET /reports/course-attendance` (basic attendance aggregation)

Standard error codes: `400/401/403/404/409/422`.

## Profile Image Upload
- Endpoint: `POST /students/{id}/image` (multipart/form‑data, key `file`)
- Files are stored under `Storage:BasePath` (e.g., `storage/`) and returned as relative URLs `/storage/<file>`.

## Running Tests
Run all tests:
```
dotnet test
```
Covers:
- Prevent duplicate Enrollment (StudentId, CourseId)
- Prevent duplicate Attendance (EnrollmentId, ClassSessionId)
- Self‑enroll uses current student (`/courses/{courseId}/enroll`)
- Pagination returns correct `total/page/pageSize`

## Docker
Docker Compose runs MSSQL 2022 + API (port 8080):
```
docker compose up -d
```
Environment variables (compose):
- `MSSQL_SA_PASSWORD` (default: `Your_password123`)
- `ConnectionStrings__Default`
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key`
- `Storage__BasePath`, `FileUpload__MaxMB`

Once running, navigate to: http://localhost:8080/swagger

## Project Tree (high‑level)
```
Academy.Crm.Api/
  Controllers/
  DTOs/
  Mapping/
  Services/
  Program.cs
  appsettings.json
Academy.Crm.Core/
  Entities/
  Enums/
  Interfaces/
Academy.Crm.Infra/
  Db/
    Configurations/
    AppDbContext.cs
  Repositories/
  Seed/
Academy.Crm.Tests/
Academy.Crm.Api/Dockerfile
docker-compose.yml
```

## Quick Commands
```
dotnet ef database update
dotnet run --project Academy.Crm.Api
```

Swagger: http://localhost:8080/swagger

