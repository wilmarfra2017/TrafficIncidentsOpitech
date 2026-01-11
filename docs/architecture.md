# Arquitectura de la solucion

## Objetivo
Diseñar e implementar una API REST en **.NET 8** para:
- Registrar siniestros viales.
- Consultarlos por **departamento**, por **rango de fechas** (from/to), o combinando ambos filtros.
- Aplicar **paginacion** en las consultas.

La API almacena como minimo:
- Identificador unico (`Id`)
- Fecha/hora del evento (`OccurredAt`)
- Departamento (`Department`)
- Ciudad (`City`)
- Tipo de siniestro (`IncidentType`)
- Vehiculos involucrados (`Vehicles`)
- Numero de victimas (`VictimCount`)
- Descripcion opcional (`Description`)

---

## Estilo arquitectonico
Se implemento una solucion basada en:

- **Clean Architecture** (separacion en capas y dependencias hacia adentro).
- **DDD (Domain-Driven Design)** con un **Aggregate Root** (`TrafficIncidentAggregate`).
- **CQRS** (Commands para escritura / Queries para lectura).
- **MediatR** como patron **Mediator** para desacoplar controllers de la logica de negocio.
- **Repository pattern** para desacoplar Application de EF Core.
- **Unit of Work** mediante `IUnitOfWork` (implementado por el DbContext).
- **FluentValidation** para validacion de Commands via pipeline behavior.
- **EF Core + SQL Server** como persistencia principal.
- **SQLite in-memory** para pruebas de integracion.

---

## Estructura por capas

### 1 - TrafficIncidentsOpitech.Api (Capa API)
Responsabilidades:
- Exponer endpoints HTTP (Controllers).
- Configuracion de DI: `AddApplication()` y `AddInfrastructure()`.
- Manejo de errores centralizado por middleware.
- Swagger/OpenAPI para exploracion y pruebas.

Componentes relevantes:
- `TrafficIncidentsController`
  - `POST /api/traffic-incidents` (registro)
  - `GET /api/traffic-incidents` (busqueda con filtros + paginacion)
- `ExceptionHandlingMiddleware`
  - Centraliza errores (validacion/argumentos/errores no controlados)
  - Evita `try/catch` repetidos en controllers

---

### 2 - TrafficIncidentsOpitech.Application (Capa Application)
Responsabilidades:
- Implementar casos de uso con CQRS:
  - **Commands**: escritura (ej. `RegisterTrafficIncidentCommand`)
  - **Queries**: lectura (ej. `GetTrafficIncidentsQuery`)
- Definir **contratos** (interfaces) que Infrastructure implementa.
- Validacion del input usando FluentValidation.
- Modelos/DTOs minimos para retornar resultados paginados.

Contratos principales:
- `ITrafficIncidentRepository` (escritura / agregado)
- `ITrafficIncidentReadRepository` (lectura / proyecciones)
- `IUnitOfWork` (commit de transacciones)

Validacion:
- `RegisterTrafficIncidentCommandValidator` valida el Command antes del handler.
- `ValidationBehavior<TRequest, TResponse>` ejecuta validadores via pipeline de MediatR.

---

### 3 - TrafficIncidentsOpitech.Domain (Capa Domain)
Responsabilidades:
- Contiene el modelo del negocio sin dependencias externas.
- Define el agregado y sus invariantes basicas.

Modelo principal:
- `TrafficIncidentAggregate`
  - Contiene la lista de `InvolvedVehicle`
  - Define comportamiento (ej. `AddVehicle(...)`)
- Enums:
  - `IncidentType`
  - `VehicleType` (`Unknown = 0` se bloquea por validacion)

---

### 4 - TrafficIncidentsOpitech.Infrastructure (Capa Infrastructure)
Responsabilidades:
- Persistencia con EF Core.
- Implementacion de repositorios definidos en Application.
- Configuracion de DbContext.
- Migraciones y acceso a SQL Server.
- Implementacion de lectura con filtros + paginacion.

Componentes relevantes:
- `TrafficIncidentsDbContext : DbContext, IUnitOfWork`
  - Implementa `SaveChangesAsync` como commit transaccional.
- `TrafficIncidentRepository` (escritura)
  - `AddAsync(...)`
  - `GetByIdAsync(...)` (incluye vehiculos)
  - expone `UnitOfWork`
- `TrafficIncidentReadRepository` (lectura)
  - `SearchAsync(criteria)` con filtros combinables y paginacion
- `ServiceCollectionExtension.AddInfrastructure(...)`
  - Registra EF Core (SQL Server)
  - Registra repositorios en DI

---

## CQRS: Escritura vs Lectura

### Escritura (Command)
Caso de uso: registrar siniestro vial.

- **Controller** recibe JSON y lo mapea a `RegisterTrafficIncidentCommand`.
- **MediatR** ejecuta pipeline:
  - `ValidationBehavior` (FluentValidation)
  - Handler
- **Handler**:
  - Crea el agregado `TrafficIncidentAggregate`
  - Agrega vehiculos con `AddVehicle(...)`
  - Guarda con repositorio de escritura y hace commit con `UnitOfWork.SaveChangesAsync()`

**Resultado**: retorna `Guid` del siniestro creado.

---

### Lectura (Query)
Caso de uso: consultar siniestros por filtros y paginacion.

- **Controller** recibe query params:
  - `department`, `from`, `to`, `page`, `pageSize`
- Se envia `GetTrafficIncidentsQuery` a MediatR.
- **Handler** usa `ITrafficIncidentReadRepository`
  - Aplica filtros opcionales (department/from/to)
  - Ordena por fecha (`OccurredAt`) descendente
  - Aplica `Skip/Take` para paginacion
  - Retorna `PagedResult<TrafficIncidentListItemDto>`

**Respuesta**: `PagedResponse<T>` con items, totalCount, page y pageSize.

---

## Unit of Work (IUnitOfWork)
Se usa para asegurar que el guardado del agregado y sus dependencias se confirme en una sola operacion de persistencia:

- `TrafficIncidentsDbContext` implementa `IUnitOfWork`
- El repositorio de escritura expone `UnitOfWork`
- El handler ejecuta el commit con `SaveChangesAsync()`

Ventajas:
- Mantiene transacciones coherentes.
- Evita que el handler dependa directamente del DbContext.

---

## Manejo de errores (Middleware)
El manejo de errores se centraliza en `ExceptionHandlingMiddleware`, evitando `try/catch` repetidos en Controllers.

Casos tipicos:
- `FluentValidation.ValidationException` => 400
- `ArgumentException / ArgumentOutOfRangeException` => 400
- Excepciones no controladas => 500 con `traceId`

## Persistencia y migraciones

La persistencia se implementa con EF Core en la capa **Infrastructure**, donde se ubica  `TrafficIncidentsDbContext`.
Las migraciones se almacenan en:

- `TrafficIncidentsOpitech.Infrastructure/Persistence/Migrations`

### Comandos (Package Manager Console)
Crear migracion:
EntityFrameworkCore\Add-Migration InitialCreate -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext -OutputDir Persistence\Migrations

Aplicar migracion:
EntityFrameworkCore\Update-Database -Project TrafficIncidentsOpitech.Infrastructure -StartupProject TrafficIncidentsOpitech.Api -Context TrafficIncidentsDbContext

---

## Pruebas

### Pruebas de Integracion (SQLite in-memory)
- Se usa `WebApplicationFactory` para levantar la API en memoria.
- Se reemplaza el DbContext configurado para SQL Server por SQLite in-memory.
- Se crea el esquema con `EnsureCreated()`.
- Se validan endpoints de consulta (GET) end-to-end.

Beneficios:
- Reproducibles, rapidas, sin depender de SQL Server local/externo.

### Pruebas unitarias (sugeridas/posibles)
- Validacion del Command (`RegisterTrafficIncidentCommandValidator`)
- Handler de registro verificando llamadas al repositorio y UnitOfWork (Moq)

---

## Flujo resumido de dependencias
- **API** depende de **Application** y **Infrastructure** (solo para registrar DI).
- **Application** depende de **Domain**.
- **Infrastructure** depende de **Application** y **Domain**.
- **Domain** no depende de nada.

Esto asegura que la logica del negocio y casos de uso permanezcan aislados de detalles tecnicos.
