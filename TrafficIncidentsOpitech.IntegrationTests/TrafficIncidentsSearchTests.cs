using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TrafficIncidentsOpitech.Domain.Enums;
using TrafficIncidentsOpitech.Domain.Incidents;
using TrafficIncidentsOpitech.Infrastructure.Persistence;

namespace TrafficIncidentsOpitech.IntegrationTests;

public sealed class TrafficIncidentsSearchTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public TrafficIncidentsSearchTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Search_ByDepartment_ReturnsOnlyThatDepartment()
    {
        // Arrange
        await SeedAsync();

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/traffic-incidents?department=Tolima&page=1&pageSize=10");

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Status: {response.StatusCode}\nBody:\n{body}");
        

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<TrafficIncidentListItemDto>>(_jsonOptions);
        Assert.NotNull(payload);

        Assert.All(payload!.Items, x => Assert.Equal("Tolima", x.Department));
        Assert.True(payload.TotalCount >= payload.Items.Count);
        Assert.Equal(1, payload.Page);
        Assert.Equal(10, payload.PageSize);
    }

    [Fact]
    public async Task Search_CombinedFilters_AndPagination_Works()
    {
        // Arrange
        await SeedAsync();

        var client = _factory.CreateClient();

        // Act (combinado + paginación)
        var response = await client.GetAsync(
            "/api/traffic-incidents?department=Tolima&from=2026-01-01T00:00:00-05:00&to=2026-01-31T23:59:59-05:00&page=1&pageSize=1");

        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Status: {response.StatusCode}\nBody:\n{body}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<PagedResponse<TrafficIncidentListItemDto>>(_jsonOptions);
        Assert.NotNull(payload);
        
        Assert.True(payload!.TotalCount >= 1);
        Assert.True(payload.Items.Count <= 1);
        Assert.Equal(1, payload.Page);
        Assert.Equal(1, payload.PageSize);
        
        Assert.All(payload.Items, x =>
        {
            Assert.Equal("Tolima", x.Department);
            Assert.True(x.OccurredAt >= DateTimeOffset.Parse("2026-01-01T00:00:00-05:00"));
            Assert.True(x.OccurredAt <= DateTimeOffset.Parse("2026-01-31T23:59:59-05:00"));
        });
    }
    
    private async Task SeedAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TrafficIncidentsDbContext>();
        
        db.TrafficIncidents.RemoveRange(db.TrafficIncidents);
        await db.SaveChangesAsync();
        
        var i1 = new TrafficIncidentAggregate(
            id: Guid.NewGuid(),
            occurredAt: DateTimeOffset.Parse("2026-01-10T10:30:00-05:00"),
            department: "Tolima",
            city: "Ibague",
            incidentType: IncidentType.Collision,
            victimCount: 2,
            description: "Test 1"
        );
        i1.AddVehicle(VehicleType.Car, "ABC123", "Car 1");

        var i2 = new TrafficIncidentAggregate(
            id: Guid.NewGuid(),
            occurredAt: DateTimeOffset.Parse("2026-01-20T08:00:00-05:00"),
            department: "Tolima",
            city: "Ibague",
            incidentType: IncidentType.RunOffRoad,
            victimCount: 0,
            description: "Test 2"
        );
        i2.AddVehicle(VehicleType.Motorcycle, "MOTO99", "Moto");

        var i3 = new TrafficIncidentAggregate(
            id: Guid.NewGuid(),
            occurredAt: DateTimeOffset.Parse("2026-02-05T12:00:00-05:00"),
            department: "Cundinamarca",
            city: "Bogota",
            incidentType: IncidentType.PedestrianHit,
            victimCount: 1,
            description: "Test 3"
        );
        i3.AddVehicle(VehicleType.Bus, "BUS777", "Bus");

        await db.TrafficIncidents.AddRangeAsync(i1, i2, i3);
        await db.SaveChangesAsync();
    }
    
    private sealed record PagedResponse<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

    private sealed record TrafficIncidentListItemDto(
        Guid Id,
        DateTimeOffset OccurredAt,
        string Department,
        string City,
        IncidentType IncidentType,
        int VictimCount,
        string? Description,
        IReadOnlyList<TrafficIncidentVehicleDto> Vehicles
    );

    private sealed record TrafficIncidentVehicleDto(VehicleType VehicleType, string? Plate, string? Notes);
}
