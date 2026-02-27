using System.Security.Claims;
using Agenda.Thome.Application.DTOs;
using Agenda.Thome.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Thome.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Lista todas as consultas do usuário autenticado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var appointments = await _appointmentService.GetAllByUserAsync(userId);
        return Ok(appointments);
    }

    /// <summary>
    /// Busca uma consulta específica por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var appointment = await _appointmentService.GetByIdAsync(userId, id);

        if (appointment is null)
            return NotFound(new { message = "Consulta não encontrada." });

        return Ok(appointment);
    }

    /// <summary>
    /// Cria uma nova consulta.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AppointmentRequest request)
    {
        try
        {
            var userId = GetUserId();
            var appointment = await _appointmentService.CreateAsync(userId, request);
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza uma consulta existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AppointmentRequest request)
    {
        try
        {
            var userId = GetUserId();
            var appointment = await _appointmentService.UpdateAsync(userId, id, request);
            return Ok(appointment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Exclui uma consulta.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = GetUserId();
            await _appointmentService.DeleteAsync(userId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

        return Guid.Parse(userIdClaim);
    }
}
