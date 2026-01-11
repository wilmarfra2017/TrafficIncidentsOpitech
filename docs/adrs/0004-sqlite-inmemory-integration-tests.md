# ADR 0004: SQLite in-memory para pruebas de integraciun

## Estado
Aceptado

## Contexto
Se solicitu que la soluciun incluya pruebas, y se valoran pruebas de integraciun.
Ejecutar tests contra SQL Server real agrega fricciun (instalaciun, credenciales, entornos, CI/CD).
Se necesitaba una base de datos rapida y aislada para validar el comportamiento de la API end-to-end.

## Decisiun
Para tests de integraciun se usa:
- **SQLite in-memory** manteniendo la conexiun abierta durante el ciclo de vida del test
- `WebApplicationFactory` para levantar la API en memoria
- Reemplazo del `DbContext` registrado (SQL Server) por uno configurado para SQLite

Se crea el esquema de base de datos en el setup de tests usando `EnsureCreated()`.

## Consecuencias
Tests de integraciun rapidos, reproducibles y sin dependencia de infraestructura externa.  
Facilita ejecuciun local y en pipelines CI.  
SQLite no es identico a SQL Server: algunas traducciones LINQ o tipos pueden diferir.  
Se deben evitar expresiones no traducibles por SQLite (o ajustar mapeos/queries).  
`EnsureCreated()` no valida migraciones; se asume consistencia de modelo para el test (en SQL Server se valida con migraciones).
