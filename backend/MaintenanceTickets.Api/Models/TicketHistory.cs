namespace MaintenanceTickets.Api.Models;

public class TicketHistory
{
    public int TicketHistoryId { get; set; }

    public int TicketId { get; set; }

    public string? PreviousStatus { get; set; }

    public string NewStatus { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Ticket Ticket { get; set; } = null!;
}