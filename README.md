# rentline_backend

Minimal **.NET 8** Web API for a landlord management SaaS (backend-only). It includes **JWT authentication**, **role-based policies**, **multi-tenant org scoping**, **Cloudinary** uploads for property images, and the principal CRUDs you’d expect (properties, units, leases, maintenance, invites, tenants).  
**Database:** PostgreSQL • **Docs:** Swagger enabled in all environments.

---

## ✨ Features

- JWT auth (register org, login, accept tenant invite)
- Roles & policies: `Landlord`, `AgencyAdmin`, `Manager`, `Maintenance`, `Tenant`, `Viewer`
- Per-request org scoping via `orgId` claim + EF Core global query filter
- CRUDs:
  - Organizations (`/api/org/me`)
  - Properties (+ Cloudinary image upload)
  - Units
  - Leases (overlap prevention by unit)
  - Maintenance (tenant creates; owner/manager manages)
  - Invites (owner/manager → tenant)
  - Tenants (list)
- Swagger UI with Bearer auth

---

## 🧱 Tech stack

- .NET 8, ASP.NET Core Web API
- EF Core + **Npgsql** (PostgreSQL)
- CloudinaryDotNet (image uploads)
- BCrypt.Net (password hashing)
- Swashbuckle (Swagger)

---

## ✅ Prereqs

- .NET 8 SDK
- PostgreSQL (local or remote)
- Cloudinary account (Cloud name, API Key, API Secret)

---

## 🚀 Setup

1. **Configure** `src/rentline_backend/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "Default": "Host=localhost;Port=5432;Database=rentline;Username=postgres;Password=postgres"
     },
     "Jwt": {
       "Issuer": "rentline-dev",
       "Audience": "rentline-dev",
       "Key": "dev-super-secret-key-change-me"
     },
     "Cloudinary": {
       "CloudName": "your-cloud-name",
       "ApiKey": "your-api-key",
       "ApiSecret": "your-api-secret"
     }
   }
   ```

2. **Create DB & run migrations**
   ```bash
   cd src/rentline_backend
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add init_pg --output-dir Data/Migrations
   dotnet ef database update
   ```

3. **Run**
   ```bash
   dotnet run
   ```
   Swagger: `https://localhost:5001/swagger`

> If you have multiple `.csproj` files, pass `--project` and `--startup-project` to `dotnet ef` with the path to `src/rentline_backend/rentline_backend.csproj`.

---

## 🔐 Auth & Policies

- **JWT claims** include:
  ```json
  {
    "sub": "<user-id>",
    "email": "user@example.com",
    "role": "Landlord | AgencyAdmin | Manager | Maintenance | Tenant | Viewer",
    "orgId": "<organization-guid>"
  }
  ```

- **Policies** (in `Program.cs`):
  - `OrgMember`: requires `orgId` claim
  - `OwnerOrManager`: Landlord/AgencyAdmin/Manager
  - `MaintenanceOrManager`: Maintenance or Owner/Manager
  - `TenantOnly`: Tenant

- **Org scoping**: middleware sets `AppDbContext.CurrentOrgId` from the JWT. EF applies a **global filter** to all entities implementing `IMultiTenant` → your reads are automatically limited to your org.

> Middleware order matters: `UseAuthentication()` → **set `CurrentOrgId`** → `UseAuthorization()`.

---

## 🔌 Auth flow (examples)

### Register organization (creates org + first admin)
```
POST /api/auth/register-org
Content-Type: application/json

{
  "orgName": "My Rentals",
  "email": "owner@example.com",
  "password": "Passw0rd!",
  "displayName": "Owner One",
  "orgType": "Landlord" // or "Agency"
}
```
**Response**
```json
{ "token": "<JWT>", "orgId": "<GUID>", "role": "Landlord" }
```

### Login
```
POST /api/auth/login
Content-Type: application/json

{ "email": "owner@example.com", "password": "Passw0rd!" }
```
**Response**
```json
{ "token": "<JWT>", "orgId": "<GUID>", "role": "Landlord" }
```

### Accept tenant invite
```
POST /api/auth/accept-invite
Content-Type: application/json

{ "token": "<invite-token>", "displayName": "John Tenant", "password": "Passw0rd!" }
```
**Response**
```json
{ "token": "<JWT>", "orgId": "<GUID>", "role": "Tenant" }
```

---

## 🧭 Core endpoints

> In Swagger, click **Authorize** and paste `Bearer <token>`.

### Organization
- `GET /api/org/me` *(OrgMember)*  
  Returns the caller’s organization (with `Properties`, `Units`, `Images` via `Include/ThenInclude`).

### Invites
- `POST /api/invites` *(OwnerOrManager)*  
  Body: `{ "email": "tenant@example.com" }`  
  Response includes `{ token, email, expiresAt }`.

### Properties
- `GET /api/properties` *(OrgMember)*
- `POST /api/properties` *(OwnerOrManager)*  
  ```json
  { "name": "Pine Apts", "street": "123 Pine", "city": "SP", "state": "SP", "postalCode": "00000", "country": "BR" }
  ```
- `PUT /api/properties/{id}` *(OwnerOrManager)*
- `DELETE /api/properties/{id}` *(OwnerOrManager)*
- `POST /api/properties/{id}/images` *(OwnerOrManager)*  
  **multipart/form-data**: `file=@my.jpg` → uploads to Cloudinary

### Units
- `GET /api/units/by-property/{propertyId}` *(OrgMember)*
- `POST /api/units` *(OwnerOrManager)*  
  ```json
  { "propertyId":"<GUID>", "unitNumber":"101", "bedrooms":2, "bathrooms":1, "areaSqm":60, "rentAmount":2500, "currency":"BRL" }
  ```
- `PUT /api/units/{id}` *(OwnerOrManager)*
- `DELETE /api/units/{id}` *(OwnerOrManager)*

### Leases
- `GET /api/leases/by-unit/{unitId}` *(OrgMember)*
- `POST /api/leases` *(OwnerOrManager)*  
  Prevents **overlapping** leases per unit.
  ```json
  { "unitId":"<GUID>", "tenantUserId":"<GUID>", "startDate":"2025-09-01", "endDate":"2026-08-31", "monthlyRent":2500 }
  ```
- `PUT /api/leases/{id}` *(OwnerOrManager)*
- `DELETE /api/leases/{id}` *(OwnerOrManager)*

### Maintenance
- `GET /api/maintenance` *(OwnerOrManager)*
- `POST /api/maintenance` *(TenantOnly)*  
  ```json
  { "unitId":"<GUID>", "title":"Leaky sink", "description":"Dripping all day" }
  ```
- `POST /api/maintenance/{id}/status` *(MaintenanceOrManager)*  
  Body: `"InProgress" | "Resolved" | "Closed"`

### Tenants
- `GET /api/tenants` *(OwnerOrManager)*

---

## 🗃️ Data model (simplified)

- `Organization (OrgType Landlord/Agency)`
- `AppUser (Role)`
- `Property` → `Unit` → `Lease (TenantUserId, Start/End)`
- `MaintenanceRequest (Unit, CreatedBy)`
- `Invite (Tenant)`

All implement `IMultiTenant` with `OrganizationId` and are filtered by `CurrentOrgId`.

---

## 🖼️ Cloudinary

Set Cloudinary credentials in `appsettings.Development.json`.  
Uploading property images:
```
POST /api/properties/{id}/images
Authorization: Bearer <JWT>
Content-Type: multipart/form-data
file=@kitchen.jpg
```
Returns the uploaded image record with `Url`.

---

## 🧪 Troubleshooting

- **401 Unauthorized**: No/invalid JWT.
- **403 Forbidden**: JWT lacks `orgId` or role doesn’t satisfy the policy.
- **Global filter crash**: Ensure middleware order (`UseAuthentication` → set `CurrentOrgId` → `UseAuthorization`) and the query filter uses `GetValueOrDefault()` (already in template).
- **Swagger “Authorize”**: Paste the token as-is (no need to type `Bearer`, the UI adds it if configured to).

---

## 📂 Structure

```
src/rentline_backend/
  Controllers/
  Data/
  Domain/
  DTOs/
  Services/
  Properties/
  Program.cs
  rentline_backend.csproj
```

---

## 🔒 Notes

- JWT keys in appsettings are for dev only. Use secrets or env vars in prod.
- EF migrations should be committed to version control.

---

Happy shipping!
