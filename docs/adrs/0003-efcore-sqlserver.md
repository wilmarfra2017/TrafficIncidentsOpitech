# ADR 0003: Persistencia con EF Core + SQL Server

## Estado
Aceptado

## Contexto
Se requiere una persistencia "tipica" en entorno .NET, con soporte de migraciones, consultas con filtros y paginacion, y un proveedor ampliamente adoptado.
SQL Server es una eleccion comun en escenarios empresariales y encaja con la necesidad del ejercicio.

## Decision
Se utiliza:
- **Entity Framework Core** como ORM
- **SQL Server** como base de datos principal
- Persistencia implementada en la capa **Infrastructure**
- Contratos (interfaces) definidos en **Application** para evitar acoplamiento

Se implementan repositorios:
- **Repositorio de escritura**: `ITrafficIncidentRepository` (Add + UnitOfWork)
- **Repositorio de lectura**: `ITrafficIncidentReadRepository` (Search con filtros/paginacion)

## Consecuencias
EF Core simplifica mapeos, migraciones y acceso a datos.  
SQL Server es estable y estandar para .NET.  
Repositorios y contratos permiten cambiar el proveedor con minimo impacto.  
EF Core implica cuidar traduccion LINQ/proveedor (diferencias entre SQL Server/SQLite).  
Requiere configuracion de conexion y herramientas de migracion (EF Tools).
