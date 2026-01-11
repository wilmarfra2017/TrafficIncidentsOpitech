using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrafficIncidentsOpitech.Domain.Incidents;

namespace TrafficIncidentsOpitech.Infrastructure.Persistence.Configurations
{
    public class TrafficIncidentAggregateConfiguration : IEntityTypeConfiguration<TrafficIncidentAggregate>
    {
        public void Configure(EntityTypeBuilder<TrafficIncidentAggregate> builder)
        {
            builder.ToTable("TrafficIncidents");
            builder.HasKey(x => x.Id);
            
            var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, DateTime>(
                v => v.UtcDateTime,
                v => new DateTimeOffset(DateTime.SpecifyKind(v, DateTimeKind.Utc))
            );

            builder.Property(x => x.OccurredAt)
                .HasConversion(dateTimeOffsetConverter);


            builder.ToTable("TrafficIncidents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OccurredAt)
                .IsRequired();

            builder.Property(x => x.Department)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(x => x.City)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(x => x.IncidentType)
                .IsRequired();

            builder.Property(x => x.VictimCount)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500)
                .IsRequired(false);
            
            builder.HasIndex(x => x.OccurredAt);
            builder.HasIndex(x => x.Department);
            builder.HasIndex(x => new { x.Department, x.OccurredAt });
            
            builder.OwnsMany(x => x.InvolvedVehicles, vehicles =>
            {
                vehicles.ToTable("TrafficIncidentVehicles");                
                vehicles.Property<Guid>("Id");
                vehicles.HasKey("Id");

                vehicles.WithOwner().HasForeignKey("TrafficIncidentId");

                vehicles.Property(v => v.VehicleType)
                    .IsRequired();

                vehicles.Property(v => v.Plate)
                    .HasMaxLength(15)
                    .IsRequired(false);

                vehicles.Property(v => v.Notes)
                    .HasMaxLength(200)
                    .IsRequired(false);

                vehicles.HasIndex("TrafficIncidentId");
            });
            
            builder.Navigation(x => x.InvolvedVehicles)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
