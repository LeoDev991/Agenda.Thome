using Agenda.Thome.Application.DTOs;
using Agenda.Thome.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Agenda.Thome.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Lista os horários disponíveis para agendamento em uma data específica.
    /// </summary>
    [HttpGet("{bookingToken:guid}/slots")]
    public async Task<IActionResult> GetAvailableSlots(Guid bookingToken, [FromQuery] DateTime date)
    {
        try
        {
            var slots = await _bookingService.GetAvailableSlotsAsync(bookingToken, date);
            return Ok(slots);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Realiza o agendamento de uma consulta pelo link público.
    /// </summary>
    [HttpPost("{bookingToken:guid}/book")]
    public async Task<IActionResult> Book(Guid bookingToken, [FromBody] BookingRequest request)
    {
        try
        {
            var appointment = await _bookingService.BookAsync(bookingToken, request);
            return Created(string.Empty, appointment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
