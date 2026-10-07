using MaintenanceTickets.Api.DTOs;
using MaintenanceTickets.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceTickets.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();
        return Ok(tickets);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(
        CreateTicketRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) ||
            string.IsNullOrWhiteSpace(request.Asset) ||
            string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Title, asset and description are required.");
        }

        var ticket = await _ticketService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = ticket.TicketId },
            ticket);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<TicketResponse>> ChangeStatus(
        int id,
        ChangeTicketStatusRequest request)
    {
        try
        {
            var ticket = await _ticketService.ChangeStatusAsync(id, request);

            return Ok(ticket);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}