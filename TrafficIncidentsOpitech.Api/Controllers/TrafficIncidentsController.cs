using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrafficIncidentsOpitech.Api.Controllers.Requests;
using TrafficIncidentsOpitech.Api.Controllers.Responses;
using TrafficIncidentsOpitech.Application.Incidents.Commands.RegisterTrafficIncident;
using TrafficIncidentsOpitech.Application.Incidents.Queries.GetTrafficIncidents;

namespace TrafficIncidentsOpitech.Api.Controllers;

[ApiController]
[Route("api/traffic-incidents")]
public sealed class TrafficIncidentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TrafficIncidentsController> _logger;

    public TrafficIncidentsController(IMediator mediator, ILogger<TrafficIncidentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegisterTrafficIncidentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterTrafficIncidentRequest request, CancellationToken ct)
    {
        var requestJson = JsonSerializer.Serialize(request);

        _logger.LogInformation("Metodo de Registro de Incidente. Payload: {Payload}", requestJson);
        
        var command = new RegisterTrafficIncidentCommand(
            OccurredAt: request.OccurredAt,
            Department: request.Department,
            City: request.City,
            IncidentType: request.IncidentType,
            VictimCount: request.VictimCount,
            Description: request.Description,
            Vehicles: (request.Vehicles ?? new List<RegisterTrafficIncidentVehicleRequest>())
            .Select(v => new RegisterTrafficIncidentVehicleDto(v.VehicleType, v.Plate, v.Notes))
            .ToList()
        );

        var id = await _mediator.Send(command, ct);

        _logger.LogInformation("Incidente de trafico registrado correctamente. Id: {Id}", id);

        return CreatedAtAction(nameof(GetById), new { id }, new RegisterTrafficIncidentResponse(id));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {        
        _logger.LogInformation("Peticion GetById. Id: {Id}", id);
        return Ok(new { Id = id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TrafficIncidentListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] string? department,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Consulta trafico de incidente -> Department: {Department}, From: {From}, To: {To}, Page: {Page}, PageSize: {PageSize}",
            department, from, to, page, pageSize);

        var result = await _mediator.Send(
            new GetTrafficIncidentsQuery(
                Department: department,
                From: from,
                To: to,
                Page: page,
                PageSize: pageSize
            ),
            ct
        );

        _logger.LogInformation(
            "Consulta completa. Retorno: {Count} items. TotalCount: {TotalCount}, Page: {Page}, PageSize: {PageSize}",
            result.Items.Count, result.TotalCount, result.Page, result.PageSize);

        var response = new PagedResponse<TrafficIncidentListItemDto>(
            result.Items,
            result.TotalCount,
            result.Page,
            result.PageSize
        );

        return Ok(response);
    }
}
