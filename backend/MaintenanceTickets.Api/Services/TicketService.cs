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
                AssignedOperator = ticket.AssignedOperator,
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
            AssignedOperator = ticket.AssignedOperator,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }

    public async Task<TicketResponse> ChangeStatusAsync(
    int ticketId,
    ChangeTicketStatusRequest request)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(ticket => ticket.TicketId == ticketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException("Ticket not found.");
        }

        if (!Enum.TryParse<TicketStatus>(
                request.NewStatus,
                true,
                out var newStatus))
        {
            throw new InvalidOperationException("Invalid ticket status.");
        }

        var validTransition =
            ticket.Status == TicketStatus.Pending &&
            newStatus == TicketStatus.InProgress
            ||
            ticket.Status == TicketStatus.InProgress &&
            newStatus == TicketStatus.Resolved;

        if (!validTransition)
        {
            throw new InvalidOperationException(
                $"Transition from {ticket.Status} to {newStatus} is not allowed.");
        }

        if (newStatus == TicketStatus.Resolved &&
            string.IsNullOrWhiteSpace(request.Diagnosis))
        {
            throw new InvalidOperationException(
                "A diagnosis is required to resolve the ticket.");
        }

        if (
            ticket.Status == TicketStatus.Pending &&
            newStatus == TicketStatus.InProgress &&
            string.IsNullOrWhiteSpace(request.AssignedOperator)
        )
        
        {
            throw new InvalidOperationException(
                "An operator is required to start progress."
            );
        }
        
        await _context.Database.ExecuteSqlInterpolatedAsync(
        $"CALL sp_change_ticket_status({ticketId}, {newStatus.ToString()}, {request.Comment}, {request.Diagnosis}, {request.AssignedOperator})"
        );

        return await _context.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.TicketId == ticketId)
            .Select(ticket => new TicketResponse
            {
                TicketId = ticket.TicketId,
                Title = ticket.Title,
                Asset = ticket.Asset,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Diagnosis = ticket.Diagnosis,
                AssignedOperator = ticket.AssignedOperator,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            })
            .FirstAsync();
    }
}