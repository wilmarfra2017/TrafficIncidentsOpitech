using TrafficIncidentsOpitech.Domain.Enums;

namespace TrafficIncidentsOpitech.Domain.Incidents
{
    public class TrafficIncidentAggregate
    {
        private readonly List<InvolvedVehicle> _involvedVehicles = new();
     
        protected TrafficIncidentAggregate() { }

        public TrafficIncidentAggregate(
            Guid id,
            DateTimeOffset occurredAt,
            string department,
            string city,
            IncidentType incidentType,
            int victimCount,
            string? description = null)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id no puede ser nulo.", nameof(id));
            if (string.IsNullOrWhiteSpace(department)) throw new ArgumentException("Departmento is requerido.", nameof(department));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("Ciudad es requerida.", nameof(city));
            if (victimCount < 0) throw new ArgumentOutOfRangeException(nameof(victimCount), "El conteo de victimas no puede ser negativo.");

            Id = id;
            OccurredAt = occurredAt;
            Department = department.Trim();
            City = city.Trim();
            IncidentType = incidentType;
            VictimCount = victimCount;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        public Guid Id { get; private set; }
        public DateTimeOffset OccurredAt { get; private set; }
        public string Department { get; private set; } = default!;
        public string City { get; private set; } = default!;
        public IncidentType IncidentType { get; private set; }
        public int VictimCount { get; private set; }
        public string? Description { get; private set; }

        public IReadOnlyCollection<InvolvedVehicle> InvolvedVehicles => _involvedVehicles.AsReadOnly();

        public void AddVehicle(VehicleType vehicleType, string? plate = null, string? notes = null)
        {            
            _involvedVehicles.Add(new InvolvedVehicle(vehicleType, plate, notes));
        }
    }

    public class InvolvedVehicle
    {      
        protected InvolvedVehicle() { }

        public InvolvedVehicle(VehicleType vehicleType, string? plate = null, string? notes = null)
        {
            VehicleType = vehicleType;
            Plate = string.IsNullOrWhiteSpace(plate) ? null : plate.Trim().ToUpperInvariant();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        public VehicleType VehicleType { get; private set; }
        public string? Plate { get; private set; }
        public string? Notes { get; private set; }
    }

}
