# ADR 0002: CQRS con MediatR

## Estado
Aceptado

## Contexto
El requerimiento exige separar los casos de uso de escritura y lectura, y se valora el uso de patrones como Mediator.
Ademas, se necesitaba un punto central para agregar comportamientos transversales (validacion, logging, etc.) sin contaminar controllers ni handlers.

## Decision
Se adopta CQRS (Command Query Responsibility Segregation) con MediatR:

- **Commands (escritura)**: encapsulan operaciones que cambian estado (ej: `RegisterTrafficIncidentCommand`).
- **Queries (lectura)**: encapsulan operaciones de consulta (ej: `GetTrafficIncidentsQuery`).
- **MediatR** actua como Mediator para desacoplar Controllers de la logica de aplicacion y de la infraestructura.

## Consecuencias
Separacion clara entre lectura y escritura, facilitando evolucion y mantenimiento.  
Controllers delgados: solo mapean HTTP => Command/Query => respuesta.  
Se habilitan behaviors transversales (ej. validacion con pipeline) sin duplicacion.  
Aumenta la cantidad de clases (Command/Query/Handler/DTOs).  
Requiere disciplina para no mezclar responsabilidades (ej. no meter EF en Controllers/Domain).
