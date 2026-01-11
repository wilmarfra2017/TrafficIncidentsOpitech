# TrafficIncidentsOpitech (.NET 8)

API REST para registrar siniestros viales y consultarlos por:
- Departamento
- Rango de fechas (from/to)
- Combinación de ambos filtros
- Paginación

## Arquitectura
Implementación basada en Clean Architecture + DDD (agregado) + CQRS (Commands/Queries) usando MediatR.
- **API**: Controllers + Middleware (manejo de errores)
- **Application**: Casos de uso (Commands/Queries), validaciones, contratos (interfaces)
- **Domain**: Modelo de dominio (Aggregate + Value Objects/Enums)
- **Infrastructure**: EF Core + repositorios + persistencia

Documentación adicional:
- Modelo de dominio: `docs/domain-model.md`
- Decisiones arquitectónicas (ADRs): `docs/adrs/`

## Requisitos
- .NET SDK 8
- SQL Server (local o Docker) para ejecución normal
- (Opcional) Visual Studio 2022 / Rider

## Configuración
Configura la cadena de conexión en `TrafficIncidentsOpitech.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TrafficIncidentsOpitechDb;User Id=sa;Password=***;TrustServerCertificate=True;"
  }
}

## Migraciones (SQL Server)

### Crear migración
```powershell
EntityFrameworkCore\Add-Migration InitialCreate -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext -OutputDir Persistence\Migrations

### Aplicar migración (crear/actualizar la base)
EntityFrameworkCore\Update-Database -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext
