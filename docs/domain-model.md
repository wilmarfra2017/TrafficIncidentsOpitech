# Modelo de Dominio

## Aggregate Root: TrafficIncidentAggregate
Representa un siniestro vial registrado en el sistema.

### Propiedades
- **Id (Guid)**: Identificador unico.
- **OccurredAt (DateTimeOffset)**: Fecha y hora del evento.
- **Department (string)**: Departamento.
- **City (string)**: Ciudad.
- **IncidentType (IncidentType)**: Tipo de siniestro.
- **VictimCount (int)**: Numero de victimas.
- **Description (string?)**: Descripcion opcional.
- **InvolvedVehicles (List<InvolvedVehicle>)**: Vehiculos involucrados.

### Comportamiento (metodos)
- **AddVehicle(...)**: Agrega un vehiculo involucrado al siniestro.

### Reglas/Invariantes
- Debe existir al menos **1** vehiculo involucrado.
- `VictimCount` no puede ser negativo.
- `Department` y `City` son obligatorios.
- En cada vehiculo: `VehicleType != Unknown`, `Plate` requerido, `Notes` requerido.

---

## Owned/Entity: InvolvedVehicle
Representa un vehiculo dentro del siniestro.

### Propiedades
- **VehicleType (VehicleType)**
- **Plate (string)**: Obligatoria.
- **Notes (string)**: Obligatoria.

---

## Enums
### IncidentType
Describe el tipo de siniestro (Collision, Rollover, etc.).

### VehicleType
Describe el tipo de vehiculo (Car, Motorcycle, Bus, etc.).
- `Unknown = 0` se considera invalido para persistir y se bloquea por validacion.
