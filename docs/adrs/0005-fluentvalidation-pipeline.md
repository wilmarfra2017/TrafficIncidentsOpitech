# ADR 0005: Validacion con FluentValidation en Pipeline Behavior

## Estado
Aceptado

## Contexto
El sistema requiere validacion consistente del request (campos obligatorios, reglas como vehiculos requeridos, etc.).
Hacer validaciones en Controllers genera duplicacion y controllers "gordos".
Se buscaba ubicar la validacion en la capa Application, alineado a Clean Architecture y CQRS.

## Decision
Se implementa:
- **FluentValidation** para validar Commands (ej. `RegisterTrafficIncidentCommandValidator`)
- Un **MediatR Pipeline Behavior** (`ValidationBehavior`) que ejecuta validators antes del handler
- El manejo de `ValidationException` se centraliza (middleware) devolviendo 400

Reglas aplicadas (ejemplos):
- `Department` y `City` requeridos
- `VictimCount >= 0`
- `Vehicles` requerido y con al menos 1 elemento
- Cada vehiculo: `VehicleType != Unknown`, `Plate` requerido, `Notes` requerido

## Consecuencias
Validacion centralizada, testeable y consistente.  
Controllers quedan enfocados en HTTP/mapeo, no en reglas.  
Se integran facilmente nuevas reglas sin tocar endpoints.  
Requiere configuracion adicional (registrar validators + behavior).  
Si el JSON es invalido (no parsea), la validacion no se ejecuta porque no se construye el Command (esto se maneja por model binding).
