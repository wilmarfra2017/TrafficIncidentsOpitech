# TrafficIncidentsOpitech (.NET 8)

API REST para registrar siniestros viales y consultarlos por:
- Departamento
- Rango de fechas (from/to)
- Combinación de ambos filtros
- Paginación

---

## Arquitectura

Implementación basada en **Clean Architecture + DDD (Aggregate Root) + CQRS (Commands/Queries)** usando **MediatR**.

Capas:
- **API**: Controllers + Middleware (manejo de errores) + Swagger
- **Application**: Casos de uso (Commands/Queries + Handlers), validaciones, contratos (interfaces)
- **Domain**: Modelo de dominio (Aggregate + Enums)
- **Infrastructure**: EF Core + repositorios + persistencia + migraciones

Documentación adicional:
- Modelo de dominio: `docs/domain-model.md`
- Arquitectura: `docs/architecture.md`
- Decisiones arquitectónicas (ADRs): `docs/adrs/`

---

## Requisitos

- .NET SDK 8
- SQL Server (local o Docker) para ejecución normal
- (Opcional) Visual Studio 2022 / Rider

---

## Configuración

Configura la cadena de conexión en `TrafficIncidentsOpitech.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TrafficIncidentsOpitechDb;User Id=sa;Password=***;TrustServerCertificate=True;"
  }
}
```
## Migraciones (EF Core + SQL Server)

Las migraciones se generan en el proyecto Infrastructure (donde vive el TrafficIncidentsDbContext) y se ejecutan usando como proyecto de arranque la API (porque allí está la configuración del host y appsettings.json).
Visual Studio (Package Manager Console)

## Crear migración
EntityFrameworkCore\Add-Migration InitialCreate -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext -OutputDir Persistence\Migrations

## Aplicar migración
EntityFrameworkCore\Update-Database -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext


