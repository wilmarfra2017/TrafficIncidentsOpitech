using FluentAssertions;
using Moq;
using TrafficIncidentsOpitech.Application.Abstractions.Persistence;
using TrafficIncidentsOpitech.Application.Incidents.Commands.RegisterTrafficIncident;
using TrafficIncidentsOpitech.Domain.Enums;
using TrafficIncidentsOpitech.Domain.Incidents;

namespace TrafficIncidentsOpitech.UnitTests.Incidents;

public sealed class RegisterTrafficIncidentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldAddAndSave_AndReturnId()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        uowMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var repoMock = new Mock<ITrafficIncidentRepository>(MockBehavior.Strict);
        repoMock.SetupGet(x => x.UnitOfWork).Returns(uowMock.Object);

        TrafficIncidentAggregate? captured = null;

        repoMock
            .Setup(x => x.AddAsync(It.IsAny<TrafficIncidentAggregate>(), It.IsAny<CancellationToken>()))
            .Callback<TrafficIncidentAggregate, CancellationToken>((i, _) => captured = i)
            .Returns(Task.CompletedTask);

        var handler = new RegisterTrafficIncidentCommandHandler(repoMock.Object);

        var command = new RegisterTrafficIncidentCommand(
            OccurredAt: DateTimeOffset.Parse("2026-01-10T10:30:00-05:00"),
            Department: "Tolima",
            City: "Ibague",
            IncidentType: IncidentType.Collision,
            VictimCount: 2,
            Description: "Unit test",
            Vehicles: new[]
            {
                new RegisterTrafficIncidentVehicleDto(VehicleType.Car, "ABC123", "Car 1"),
                new RegisterTrafficIncidentVehicleDto(VehicleType.Motorcycle, "MOTO99", "Moto"),
            }
        );

        // Act
        var id = await handler.Handle(command, CancellationToken.None);

        // Assert
        id.Should().NotBe(Guid.Empty);

        captured.Should().NotBeNull();
        captured!.Department.Should().Be("Tolima");
        captured.City.Should().Be("Ibague");
        captured.VictimCount.Should().Be(2);
        captured.IncidentType.Should().Be(IncidentType.Collision);
        captured.OccurredAt.Should().Be(command.OccurredAt);
        captured.Description.Should().Be("Unit test");

        captured.InvolvedVehicles.Should().HaveCount(2);

        repoMock.Verify(x => x.AddAsync(It.IsAny<TrafficIncidentAggregate>(), It.IsAny<CancellationToken>()), Times.Once);
        uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        repoMock.VerifyNoOtherCalls();
        uowMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenVehiclesIsEmpty_ShouldThrow_AndNotPersist()
    {
        // Arrange
        var uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var repoMock = new Mock<ITrafficIncidentRepository>(MockBehavior.Strict);
        repoMock.SetupGet(x => x.UnitOfWork).Returns(uowMock.Object);

        var handler = new RegisterTrafficIncidentCommandHandler(repoMock.Object);

        var command = new RegisterTrafficIncidentCommand(
            OccurredAt: DateTimeOffset.Parse("2026-01-10T10:30:00-05:00"),
            Department: "Tolima",
            City: "Ibague",
            IncidentType: IncidentType.Collision,
            VictimCount: 2,
            Description: "Unit test",
            Vehicles: Array.Empty<RegisterTrafficIncidentVehicleDto>()
        );

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*Al menos un vehiculo*");
        
        repoMock.Verify(x => x.AddAsync(It.IsAny<Domain.Incidents.TrafficIncidentAggregate>(), It.IsAny<CancellationToken>()), Times.Never);
        uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        repoMock.VerifyNoOtherCalls();
        uowMock.VerifyNoOtherCalls();
    }
}
