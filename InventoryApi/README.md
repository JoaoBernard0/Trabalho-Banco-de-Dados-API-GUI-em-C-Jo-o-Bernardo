# Trabalho: Banco de Dados + API + GUI em C#
# Nome: João Bernardo
# RA: 22300693

Stack: .NET 9, ASP.NET Core Web API, EF Core 9 + SQLite, WPF (GUI)

## Objetivo
Construir uma API REST + GUI com persistência (CRUD completo), validações e relacionamento entre ao menos 2 entidades.

## Entidades
- Category: `Id`, `Name*` (único)
- Product: `Id`, `Name*`, `Price`, `CategoryId` (FK), `CreatedAt`

## Endpoints principais
- `GET /api/products` (filtro opcional `?name=...`)
- `GET /api/products/{id}`
- `POST /api/products`
- `PUT /api/products/{id}`
- `DELETE /api/products/{id}`

- `GET /api/categories`
- `GET /api/categories/{id}`
- `POST /api/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}` (retorna 422 se tiver produtos)

## Validações
- DataAnnotations: `[Required]`, `[StringLength]`, `[Range]`
- Unique index em `Category.Name` (no `OnModelCreating`)
- Erros: 400 (bad request), 404 (not found), 409 (conflict), 422 (business rules)

## Como rodar (passo-a-passo)

### Pré-requisitos
- .NET 9 SDK
- dotnet-ef (opcional: `dotnet tool install --global dotnet-ef`)

### 1) Rodar API (InventoryApi)
```bash
cd InventoryApi
dotnet restore

# Criar migration (na primeira vez)
dotnet ef migrations add InitialCreate

# Aplicar migrations (gera inventory.db)
dotnet ef database update

# Rodar a API
dotnet run
