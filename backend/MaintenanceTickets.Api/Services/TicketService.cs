using MaintenanceTickets.Api.Data;
using MaintenanceTickets.Api.DTOs;
using MaintenanceTickets.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceTickets.Api.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TicketResponse>> GetAllAsync()
    {
        return await _context.Tickets
            .OrderByDescending(ticket => ticket.CreatedAt)
            .Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Asset = ticket.Asset,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Diagnosis = ticket.Diagnosis,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request)
    {
        var ticket = new Ticket
        {
            Title = request.Title,
            Asset = request.Asset,
            Description = request.Description,
            Status = TicketStatus.Pending
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return new TicketResponse
        {
            TicketId = ticket.TicketId,
            Title = ticket.Title,
            Asset = ticket.Asset,
            Description = ticket.Description,
            Status = ticket.Status.ToString(),
            Diagnosis = ticket.Diagnosis,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}